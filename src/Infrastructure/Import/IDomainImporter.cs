namespace Infrastructure.Import
{
    public interface IDomainImporter<T> where T : class
    {
        IEnumerable<T> FromJson(string json);
        IEnumerable<T> FromCsv(string csvContent);
    }
}