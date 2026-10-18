using System.Text.Json.Serialization;
namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Абстрактна особа, що співпрацює з видавництвом (автор, редактор).
    /// </summary>
    public abstract class Person : EntityBase
    {
        private const int MaxNameLength = 50;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set => _firstName = ValidateName(value, "Ім'я");
        }

        public string LastName
        {
            get => _lastName;
            set => _lastName = ValidateName(value, "Прізвище");
        }

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email не може бути порожнім.");

                string trimmed = value.Trim();
                int atIndex = trimmed.IndexOf('@');
                if (atIndex <= 0 || atIndex != trimmed.LastIndexOf('@')
                    || !trimmed.Substring(atIndex + 1).Contains('.')
                    || trimmed.EndsWith('.') || trimmed.Contains(' '))
                    throw new ArgumentException($"Некоректний email: «{value}».");

                _email = trimmed;
            }
        }

        /// <summary>
        /// Повне ім'я особи.
        /// </summary>
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Роль особи у видавництві.
        /// </summary>
        [JsonIgnore]
        public abstract string Role { get; }

        private static string ValidateName(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} не може бути порожнім.");

            string trimmed = value.Trim();
            if (trimmed.Length > MaxNameLength)
                throw new ArgumentException($"{fieldName} не може бути довшим за {MaxNameLength} символів.");
            if (!trimmed.All(c => char.IsLetter(c) || c == '-' || c == '\'' || c == ' '))
                throw new ArgumentException($"{fieldName} може містити лише літери, дефіс та апостроф.");

            return trimmed;
        }
    }
}
