using Infrastructure.Repositories;

namespace Infrastructure.Import
{
    public abstract class FileImporterBase<T>(IRepository<T> repository) : IFileImporter<T> where T : class
    {
        protected string Name = typeof(FileImporterBase<T>).Name;

        public async Task ImportAsync(string filePath)
        {
            string content = await File.ReadAllTextAsync(filePath);
            IEnumerable<T> range = Parse(content);
            await SaveAsync(range);
        }

        protected abstract IEnumerable<T> Parse(string content);

        protected virtual async Task SaveAsync(IEnumerable<T> range)
        {
            // Апдейтим, если совпал ключ
            foreach (T item in range)
            {
                await repository.UpdateAsync(item);
            }
        }
    }
}