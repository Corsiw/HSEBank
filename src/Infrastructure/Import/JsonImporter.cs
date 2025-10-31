using Domain.Exceptions;
using Infrastructure.Repositories;
using System.Text.Json;

namespace Infrastructure.Import
{
    [Importer(".json")]
    public class JsonImporter<T>(IRepository<T> repository) : FileImporterBase<T>(repository) where T : class
    {
        protected override IEnumerable<T> Parse(string content)
        {
            try
            {
                return JsonSerializer.Deserialize<List<T>>(content)
                       ?? Enumerable.Empty<T>();
            }
            catch (Exception ex) when (ex is FormatException or JsonException or InvalidOperationException)
            {
                throw new ImportException(ex.Message);
            }
        }
    }
}