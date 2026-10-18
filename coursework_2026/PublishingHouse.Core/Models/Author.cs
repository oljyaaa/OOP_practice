namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Автор рукопису. Отримує роялті — відсоток від виручки тиражу.
    /// </summary>
    public class Author : Person
    {
        public const decimal MaxRoyaltyPercent = 50m;

        private decimal _royaltyPercent;

        /// <summary>
        /// Відсоток роялті від виручки (0–50).
        /// </summary>
        public decimal RoyaltyPercent
        {
            get => _royaltyPercent;
            set
            {
                if (value < 0 || value > MaxRoyaltyPercent)
                    throw new ArgumentOutOfRangeException(nameof(RoyaltyPercent),
                        $"Роялті має бути в межах 0–{MaxRoyaltyPercent}%.");
                _royaltyPercent = value;
            }
        }

        public override string Role => "Автор";

        public override string GetSummary() =>
            $"{FullName} <{Email}>, роялті {RoyaltyPercent}%";
    }
}
