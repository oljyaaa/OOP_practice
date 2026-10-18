using PublishingHouse.Core.Interfaces;
using PublishingHouse.Core.Models;
using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Core.Services
{
    /// <summary>
    /// Калькуляція собівартості тиражу та прогнозованого прибутку.
    /// Постійні витрати = редагування (ставка редактора × сторінки) + верстка + дизайн обкладинки.
    /// Змінні витрати = тираж × (сторінки × (папір + друк) + обкладинка).
    /// Виручка = тираж × прогноз продажу × ціна; роялті = виручка × відсоток автора.
    /// </summary>
    public class CostCalculator : ICostCalculator
    {
        public const decimal LayoutCostPerPage = 15m;
        public const decimal CoverDesignCost = 5000m;
        public const decimal PrintingCostPerPage = 0.20m;

        public decimal CalculatePaperCostPerPage(PaperType paperType) => paperType switch
        {
            PaperType.Newsprint => 0.15m,
            PaperType.Offset => 0.30m,
            PaperType.Coated => 0.60m,
            _ => throw new ArgumentOutOfRangeException(nameof(paperType), "Невідомий тип паперу.")
        };

        public decimal CalculateCoverCostPerCopy(CoverType coverType) => coverType switch
        {
            CoverType.Paperback => 12m,
            CoverType.Hardcover => 45m,
            _ => throw new ArgumentOutOfRangeException(nameof(coverType), "Невідомий тип обкладинки.")
        };

        public PrintRunEstimate Estimate(PrintRun printRun, Manuscript manuscript, Author author, Editor? editor)
        {
            ArgumentNullException.ThrowIfNull(printRun);
            ArgumentNullException.ThrowIfNull(manuscript);
            ArgumentNullException.ThrowIfNull(author);

            int pages = manuscript.PageCount;
            decimal editingCost = editor is null ? 0m : editor.RatePerPage * pages;
            decimal fixedCosts = editingCost + LayoutCostPerPage * pages + CoverDesignCost;

            decimal costPerCopy = pages * (CalculatePaperCostPerPage(printRun.PaperType) + PrintingCostPerPage)
                                  + CalculateCoverCostPerCopy(printRun.CoverType);
            decimal variableCosts = printRun.Quantity * costPerCopy;

            decimal soldCopies = printRun.Quantity * printRun.ExpectedSellThroughPercent / 100m;
            decimal revenue = soldCopies * printRun.RetailPrice;
            decimal royalty = revenue * author.RoyaltyPercent / 100m;

            return new PrintRunEstimate(
                Math.Round(fixedCosts, 2),
                Math.Round(variableCosts, 2),
                printRun.Quantity,
                Math.Round(revenue, 2),
                Math.Round(royalty, 2));
        }
    }
}
