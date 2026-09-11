namespace Lab31.Domain;

public interface IPersonFactoryRegistry
{
    Person Create(PersonRecord record);
}
