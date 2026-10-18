namespace PublishingHouse.Core.Interfaces
{
    /// <summary>
    /// Об'єкт, що проходить рецензування.
    /// </summary>
    public interface IReviewable
    {
        /// <summary>
        /// Чи перебуває об'єкт на етапі, коли його можна рецензувати.
        /// </summary>
        bool CanBeReviewed { get; }

        /// <summary>
        /// Висновок рецензента.
        /// </summary>
        string ReviewComment { get; set; }
    }
}
