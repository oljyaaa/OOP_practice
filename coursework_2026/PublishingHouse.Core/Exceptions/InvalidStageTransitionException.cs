using PublishingHouse.Core.Models.Enums;

namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Спроба перевести рукопис на недопустимий етап підготовки.
    /// </summary>
    public class InvalidStageTransitionException : PublishingException
    {
        public ManuscriptStage From { get; }

        public ManuscriptStage To { get; }

        public InvalidStageTransitionException(ManuscriptStage from, ManuscriptStage to)
            : base($"Неможливо перевести рукопис з етапу {from} на етап {to}.")
        {
            From = from;
            To = to;
        }
    }
}
