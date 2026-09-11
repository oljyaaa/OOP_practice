namespace Lab31.Domain;

public interface IPersonFactory
{
    string RecordTypeName { get; }

    Person Create(PersonRecord record);
}
