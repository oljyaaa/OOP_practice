namespace PublishingHouse.Core.Exceptions
{
    /// <summary>
    /// Запис із вказаним ідентифікатором не знайдено.
    /// </summary>
    public class EntityNotFoundException : PublishingException
    {
        public string EntityName { get; }

        public Guid EntityId { get; }

        public EntityNotFoundException(string entityName, Guid entityId)
            : base($"{entityName} з ідентифікатором {entityId} не знайдено.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
