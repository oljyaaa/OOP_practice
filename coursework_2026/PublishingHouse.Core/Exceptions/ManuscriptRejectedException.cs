namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Рукопис не пройшов рецензування.
    /// </summary>
    public class ManuscriptRejectedException : PublishingException
    {
        public Guid ManuscriptId { get; }

        public string ManuscriptTitle { get; }

        public string Reason { get; }

        public ManuscriptRejectedException(Guid manuscriptId, string manuscriptTitle, string reason)
            : base($"Рукопис «{manuscriptTitle}» відхилено за результатами рецензування. Причина: {reason}")
        {
            ManuscriptId = manuscriptId;
            ManuscriptTitle = manuscriptTitle;
            Reason = reason;
        }
    }
}
