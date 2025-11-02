using Application.Interfaces;
using Application.Profiles;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Infrastructure.Import
{
    public class CsvImporter<TDomain, TDto>(
        IRepository<TDomain> repository,
        IImportProfile<TDomain, TDto> profile)
        : FileImporterBase<TDomain, TDto>(repository, profile)
        where TDomain : class
    {
        public override string FileExtension => ".csv";

        protected override IEnumerable<TDto> Parse(string content)
        {
            using StringReader reader = new(content);
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using CsvReader csv = new(reader, config);
            return csv.GetRecords<TDto>().ToList();
        }
    }
}