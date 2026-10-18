namespace PublishingHouse.Core.Models
{
    /// <summary>
    /// Знімок усіх даних видавництва для збереження у файл.
    /// </summary>
    public class PublishingData
    {
        public List<Author> Authors { get; set; } = new();

        public List<Editor> Editors { get; set; } = new();

        public List<Manuscript> Manuscripts { get; set; } = new();

        public List<PrintRun> PrintRuns { get; set; } = new();
    }
}
