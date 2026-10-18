namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Базове виключення бізнес-логіки видавництва.
    /// Дозволяє UI перехоплювати всі доменні помилки одним catch.
    /// </summary>
    public class PublishingException : Exception
    {
        public PublishingException(string message)
            : base(message)
        {
        }

        public PublishingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
