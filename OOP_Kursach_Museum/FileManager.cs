using System.Collections.Generic;
using System.IO;

namespace OOP_Kursach_Conferense
{
    /// <summary>
    /// Статический класс для управления файлами, содержащими данные о музейных экспонатах.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Путь к файлу с данными.
        /// </summary>
        private static string filePath = "input.txt";

        /// <summary>
        /// Считывает данные из файла и возвращает список музейных экспонатов.
        /// </summary>
        /// <returns>Список музейных экспонатов.</returns>
        public static List<Conferense> ReadFromFile()
        {
            List<Conferense> Conferenses = new List<Conferense>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4 && int.TryParse(parts[0], out int id))
                    
                        Conferenses.Add(new Conferense(id, parts[1], parts[2], parts[3]));
                    
                }
            }
            return Conferenses;
        }

        /// <summary>
        /// Записывает список музейных экспонатов в файл, перезаписывая его.
        /// </summary>
        /// <param name="conferences">Список музейных экспонатов для записи в файл.</param>
        public static void WriteToFile(List<Conferense> conferenses)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (var conf in conferenses)
                {
                    sw.WriteLine($"{conf.Id}|{conf.Name}|{conf.Role}|{conf.Sphere}");
                }
            }
        }

        /// <summary>
        /// Добавляет один музейный экспонат в файл.
        /// </summary>
        /// <param name="conf">Музейный экспонат для добавления в файл.</param>
        public static void AppendToFile(Conferense conf)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{conf.Id}|{conf.Name} | {conf.Role} | {conf.Sphere}");
            }
        }

        /// <summary>
        /// Удаляет файл с данными.
        /// </summary>
        public static void DeleteFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
