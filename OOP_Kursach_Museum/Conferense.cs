using System;

namespace OOP_Kursach_Conferense
{
    /// <summary>
    /// Структура, представляющая участника/гостя конференции.
    /// </summary>
    public struct Conferense
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Получает или задает имя человека.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Получает или задает роль человека (студент/аспирант/преподаватель/гость).
        /// </summary>
        public string Sphere { get; set; }

        /// <summary>
        /// Получает или задает тематику.
        /// </summary>

        /// <summary>
        /// Инициализирует новый экземпляр структуры <see cref="Conferense"/>.
        /// </summary>
        /// <param name="id">Имя человека.</param>
        /// <param name="name">Имя человека.</param>
        /// <param name="role">Роль человека (студент/аспирант/преподаватель/гость).</param>
        /// <param name="sphere">Тематика.</param>
        public Conferense(int id, string name, string role, string sphere)
        {
            Id = id;
            Name = name;
            Role = role;
            Sphere = sphere;
        }
    }
}
