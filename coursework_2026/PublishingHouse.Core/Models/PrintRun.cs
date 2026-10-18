using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Тираж книги, надрукований за рукописом.
    /// Допустимий розмір тиражу перевіряє сервіс (InvalidPrintRunSizeException).
    /// </summary>
    public class PrintRun : EntityBase
    {
        public const decimal MaxRetailPrice = 10000m;

        private Guid _manuscriptId;
        private int _quantity;
        private decimal _retailPrice;
        private decimal _expectedSellThroughPercent = 80m;

        public Guid ManuscriptId
        {
            get => _manuscriptId;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Тираж повинен бути прив'язаний до рукопису.");
                _manuscriptId = value;
            }
        }

        /// <summary>
        /// Кількість примірників.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Quantity),
                        "Кількість примірників має бути додатною.");
                _quantity = value;
            }
        }

        public PaperType PaperType { get; set; } = PaperType.Offset;

        public CoverType CoverType { get; set; } = CoverType.Paperback;

        /// <summary>
        /// Роздрібна ціна одного примірника, грн.
        /// </summary>
        public decimal RetailPrice
        {
            get => _retailPrice;
            set
            {
                if (value <= 0 || value > MaxRetailPrice)
                    throw new ArgumentOutOfRangeException(nameof(RetailPrice),
                        $"Ціна має бути в межах (0; {MaxRetailPrice}] грн.");
                _retailPrice = value;
            }
        }

        /// <summary>
        /// Прогнозований відсоток проданих примірників (0–100).
        /// </summary>
        public decimal ExpectedSellThroughPercent
        {
            get => _expectedSellThroughPercent;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(ExpectedSellThroughPercent),
                        "Прогноз продажу має бути в межах 0–100%.");
                _expectedSellThroughPercent = value;
            }
        }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public override string GetSummary() =>
            $"Тираж {Quantity} прим. ({CoverType}, {PaperType}), ціна {RetailPrice} грн, прогноз продажу {ExpectedSellThroughPercent}%";
    }
}
