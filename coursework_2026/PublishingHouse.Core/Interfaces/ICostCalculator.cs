using PublishingHouse.Core.Models;

namespace PublishingHouse.Core.Interfaces
{
    /// <summary>
    /// Калькуляція собівартості тиражу та прогнозованого прибутку.
    /// </summary>
    public interface ICostCalculator
    {
        /// <param name="printRun">Тираж, що розраховується.</param>
        /// <param name="manuscript">Рукопис тиражу (кількість сторінок).</param>
        /// <param name="author">Автор (роялті).</param>
        /// <param name="editor">Редактор (вартість редагування), якщо призначений.</param>
        PrintRunEstimate Estimate(PrintRun printRun, Manuscript manuscript, Author author, Editor? editor);
    }
}
