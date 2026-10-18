namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Розмір тиражу виходить за допустимі межі.
    /// </summary>
    public class InvalidPrintRunSizeException : PublishingException
    {
        public int RequestedSize { get; }

        public int MinSize { get; }

        public int MaxSize { get; }

        public InvalidPrintRunSizeException(int requestedSize, int minSize, int maxSize)
            : base($"Недопустимий розмір тиражу: {requestedSize} прим. Дозволено від {minSize} до {maxSize} прим.")
        {
            RequestedSize = requestedSize;
            MinSize = minSize;
            MaxSize = maxSize;
        }
    }
}
