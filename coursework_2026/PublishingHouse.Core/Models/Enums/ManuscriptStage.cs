namespace PublishingHouse.Core.Models.Enums
{
    /// <summary>
    /// Етапи підготовки рукопису до друку.
    /// </summary>
    public enum ManuscriptStage
    {
        Submitted,
        UnderReview,
        Editing,
        Proofreading,
        Layout,
        ReadyForPrint,
        Printed,
        Rejected
    }
}
