using Infrastructure.Repositories;
using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;
using Domain.Exceptions;

namespace Infrastructure.Import
{
    [Importer(".csv")]
    public class CsvImporter<T>(IRepository<T> repository) : FileImporterBase<T>(repository) where T : class
    {
        protected override IEnumerable<T> Parse(string content)
        {
            try
            {
                using StringReader reader = new(content);
                CsvConfiguration csvConfig = new(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
                using CsvReader csv = new(reader, csvConfig);
                return csv.GetRecords<T>().ToList();
            }
            catch (FormatException ex)
            {
                throw new ImportException(ex.Message);
            }
            
        }
    }
}