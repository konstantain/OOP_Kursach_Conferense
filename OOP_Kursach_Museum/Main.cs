using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OOP_Kursach_Conferense
{
    /// <summary>
    /// Основная форма приложения.
    /// </summary>
    public partial class Main : Form
    {
        private List<Conferense> users = new List<Conferense>();
        private ContextMenuStrip contextMenuStrip;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            buttonDeleteDatabase.Click += buttonDeleteDatabase_Click;
            button2.Click += button2_Click; // Добавляем обработчик для button2
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            textBox1.TextChanged += TextBox1_TextChanged;
            ComboBoxFilterByName.SelectedIndexChanged += FilterComboBoxes_Changed;
            ComboBoxFilterByYear.SelectedIndexChanged += FilterComboBoxes_Changed;
            

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Удалить").Click += Delete_Click;
            dataGridView1.ContextMenuStrip = contextMenuStrip;
            dataGridView1.MouseDown += DataGridView1_MouseDown;
            comboBox1.SelectedIndex = 3;
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DataTable table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("имя", typeof(string));
            table.Columns.Add("роль", typeof(string));
            table.Columns.Add("тематика", typeof(string));

            users = FileManager.ReadFromFile();
            foreach (var user in users)
            {
                table.Rows.Add(user.Id, user.Name, user.Role, user.Sphere);
            }

            dataGridView1.DataSource = table;
            dataGridView1.Update();
            UpdateFilterComboBoxes();
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет значения в комбобоксах фильтров.
        /// </summary>
        private void UpdateFilterComboBoxes()
        {
            ComboBoxFilterByName.Items.Clear();
            ComboBoxFilterByName.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterByName.Items.Contains(user.Name))
                {
                    ComboBoxFilterByName.Items.Add(user.Name);
                }
            }

            ComboBoxFilterByYear.Items.Clear();
            ComboBoxFilterByYear.Items.Add("");
            foreach (var user in users)
            {
                if (!ComboBoxFilterByYear.Items.Contains(user.Sphere.ToString()))
                {
                    ComboBoxFilterByYear.Items.Add(user.Sphere.ToString());
                }
            }
        }

        /// <summary>
        /// Обработчик клика по ячейке DataGridView.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBoxName.Text = row.Cells["name"].Value.ToString();
                comboBox1.Text = row.Cells["role"].Value.ToString();
                textBoxSphere.Text = row.Cells["sphere"].Value.ToString();
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке добавления пользователя.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { comboBox1.Focus(); }
            string name = textBoxName.Text;
            string sphere = textBoxSphere.Text;
            string role = comboBox1.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(sphere))
            {
                MessageBox.Show("поля не могут быть пустыми.");
                return;
            }
            int rc = dataGridView1.RowCount + 1;
            var conference = new Conferense(rc, name, role, sphere);
            users.Add(conference);
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Add(rc, name, role, sphere);
            FileManager.AppendToFile(conference);
            UpdateExhibitCount();
            UpdateFilterComboBoxes();
        }

        
        /// <summary>
        /// Записывает данные пользователей в файл.
        /// </summary>
        private void WriteDataToFile()
        {
            FileManager.WriteToFile(users);
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения фильтров.
        /// </summary>
        private void FilterComboBoxes_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик изменения текста в текстовом поле поиска.
        /// </summary>
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Применяет фильтры к списку пользователей.
        /// </summary>
        private void ApplyFilters()
        {
            string filterByName = ComboBoxFilterByName.Text.Trim();
            string filterByYearText = ComboBoxFilterByYear.Text.Trim();
            

            var filteredUsers = users;
            if (!string.IsNullOrWhiteSpace(filterByName))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(filterByName)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filterByYearText))
            {
                filteredUsers = filteredUsers.Where(u => u.Sphere == filterByYearText).ToList();
            }

            string searchText = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(searchText) || u.Sphere.ToString().Contains(searchText)).ToList();
            }

            UpdateData(filteredUsers);
            UpdateExhibitCount();
        }

        /// <summary>
        /// Обновляет данные в DataGridView.
        /// </summary>
        private void UpdateData(List<Conferense> filteredUsers)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            foreach (var user in filteredUsers)
            {
                table.Rows.Add(user.Id, user.Name, user.Role, user.Sphere);
            }
        }

        /// <summary>
        /// Обновляет количество гостей и участников.
        /// </summary>
        private void UpdateExhibitCount()
        {
            int count_guests = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Selected = false;
                if (dataGridView1.Rows[i].Cells[2].Value.ToString().Contains("Гость"))
                {
                    ++count_guests;
                }
            }
            
            labelCount.Text = $"Количество участников: {dataGridView1.RowCount-count_guests}";
            //int exhibitCount = users.Count(u => u.OnExhibit);
            labelExhibitCount.Text = $"Количество гостей: {count_guests}";
        }

        /// <summary>
        /// Обработчик клика по кнопке удаления базы данных.
        /// </summary>
        private void buttonDeleteDatabase_Click(object sender, EventArgs e)
        {
            users.Clear();
            DataTable table = (DataTable)dataGridView1.DataSource;
            table.Rows.Clear();
            FileManager.DeleteFile();
            UpdateExhibitCount();
            UpdateFilterComboBoxes();
        }

        /// <summary>
        /// Обработчик изменения значения ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int id = (int)dataGridView1.Rows[e.RowIndex].Cells["id"].Value;
                string name = dataGridView1.Rows[e.RowIndex].Cells["имя"].Value.ToString();
                string role = dataGridView1.Rows[e.RowIndex].Cells["роль"].Value.ToString();
                string sphere = dataGridView1.Rows[e.RowIndex].Cells["тематика"].Value.ToString();

                users[e.RowIndex] = new Conferense(id, name, role, sphere);
                WriteDataToFile();
                UpdateExhibitCount();
            }
        }

        /// <summary>
        /// Обработчик изменения состояния редактируемой ячейки DataGridView.
        /// </summary>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Обработчик нажатия правой кнопкой мыши на DataGridView.
        /// </summary>
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    contextMenuStrip.Show(dataGridView1, e.Location);
                }
            }
        }

        /// <summary>
        /// Обработчик клика по пункту контекстного меню "Удалить".
        /// </summary>
        private void Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                users.RemoveAt(index);
                dataGridView1.Rows.RemoveAt(index);
                WriteDataToFile();
                UpdateExhibitCount();
                UpdateFilterComboBoxes();
            }
        }

        /// <summary>
        /// Обработчик клика по кнопке редактирования экспоната.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                int selectedIndex = dataGridView1.CurrentRow.Index;
                if (string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(textBoxSphere.Text))
                {
                    MessageBox.Show("поля не могут быть пустыми.");
                    return;
                }
                // Обновление DataTable
                DataTable table = (DataTable)dataGridView1.DataSource;

                table.Rows[selectedIndex][1] = textBoxName.Text;
                table.Rows[selectedIndex][2] = comboBox1.Text;
                table.Rows[selectedIndex][3] = textBoxSphere.Text;
                int rc = (int)table.Rows[selectedIndex]["id"];
                // Обновление списка users
                users[selectedIndex] = new Conferense(rc, textBoxName.Text, comboBox1.Text, textBoxSphere.Text);

                // Запись обновленных данных обратно в файл
                WriteDataToFile();

                // Обновление количества экспонатов
                UpdateExhibitCount();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для редактирования.");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
