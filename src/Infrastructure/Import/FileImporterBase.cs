using Application.Interfaces;
using Application.Profiles;
using Domain.Exceptions;

namespace Infrastructure.Import
{
    public abstract class FileImporterBase<TDomain, TDto>(
        IRepository<TDomain> repository,
        IImportProfile<TDomain, TDto> profile)
        : IFileImporter
        where TDomain : class
    {
        protected readonly IRepository<TDomain> Repository = repository;
        protected readonly IImportProfile<TDomain, TDto> Profile = profile;

        public abstract string FileExtension { get; }

        public async Task ImportAsync(string content)
        {
            try
            {
                IEnumerable<TDto> dtos = Parse(content);
                IEnumerable<TDomain> entities = Profile.Map(dtos);
                await Repository.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                throw new ImportException($"Ошибка импорта ({FileExtension}): {ex.Message}", ex);
            }
        }

        protected abstract IEnumerable<TDto> Parse(string content);
    }
}
