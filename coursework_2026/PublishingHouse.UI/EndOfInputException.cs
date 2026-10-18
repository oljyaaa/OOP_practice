namespace PublishingHouse.UI
{
    /// <summary>
    /// Потік вводу закрито (Ctrl+D / кінець файлу) — застосування має коректно завершитися.
    /// </summary>
    internal sealed class EndOfInputException : Exception
    {
        public EndOfInputException()
            : base("Потік вводу завершено.")
        {
        }
    }
}
