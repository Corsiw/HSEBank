using Infrastructure.Repositories;

namespace Infrastructure.Import
{
    public abstract class FileImporterBase<T>(IRepository<T> repository) : IFileImporter<T> where T : class
    {
        public abstract string FileExtension { get; }

        public async Task ImportAsync(string filePath)
        {
            string content = await File.ReadAllTextAsync(filePath);
            IEnumerable<T> entities = Parse(content);
            await SaveAsync(entities);
        }

        protected abstract IEnumerable<T> Parse(string content);

        protected virtual async Task SaveAsync(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                await repository.UpsertAsync(item);
            }
        }
    }
}