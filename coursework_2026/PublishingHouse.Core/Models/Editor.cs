using System.Text.Json.Serialization;
using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Редактор, що працює з рукописом. Оплата — за кожну сторінку.
    /// </summary>
    public class Editor : Person
    {
        public const decimal MaxRatePerPage = 1000m;

        private decimal _ratePerPage;

        /// <summary>
        /// Ставка редактора за одну сторінку, грн.
        /// </summary>
        public decimal RatePerPage
        {
            get => _ratePerPage;
            set
            {
                if (value <= 0 || value > MaxRatePerPage)
                    throw new ArgumentOutOfRangeException(nameof(RatePerPage),
                        $"Ставка за сторінку має бути в межах (0; {MaxRatePerPage}] грн.");
                _ratePerPage = value;
            }
        }

        /// <summary>
        /// Жанр, на якому спеціалізується редактор.
        /// </summary>
        public Genre Specialization { get; set; }

        [JsonIgnore]
        public override string Role => "Редактор";

        public override string GetSummary() =>
            $"{FullName} <{Email}>, {Specialization}, {RatePerPage} грн/стор.";
    }
}
