using Application.Interfaces;
using Application.Profiles;
using System.Text.Json;

namespace Infrastructure.Import
{
    public class JsonImporter<TDomain, TDto>(
        IRepository<TDomain> repository,
        IImportProfile<TDomain, TDto> profile)
        : FileImporterBase<TDomain, TDto>(repository, profile)
        where TDomain : class
    {
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
        public override string FileExtension => ".json";

        protected override IEnumerable<TDto> Parse(string content)
        {
            List<TDto>? dtos = JsonSerializer.Deserialize<List<TDto>>(content, _options);
            return dtos ?? [];
        }
    }
}