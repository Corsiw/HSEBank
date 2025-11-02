using Application.Interfaces;
using Domain.Common;
using Domain.Exceptions;

namespace Application.Services
{
    public class ImportService(IEnumerable<IFileImporter> importers) : IImportService
    {
        private readonly IReadOnlyCollection<IFileImporter> _importers = importers.ToList().AsReadOnly();

        public IReadOnlyCollection<(string type, string extension)> GetTypeExtensionOptions()
        {
            return _importers
                .Select(t => (t.GetType().GenericTypeArguments[0].Name, t.FileExtension))
                .Distinct()
                .ToList()
                .AsReadOnly();
        }

        public async Task<Result> ImportFileByTypeAsync(string filePath, string typeName)
        {
            filePath = filePath.Trim();
            if (!File.Exists(filePath))
            {
                return Result.Fail($"Файл {filePath} не найден.");
            }
            
            string ext = Path.GetExtension(filePath).ToLower();

            IFileImporter? descriptor = _importers.FirstOrDefault(i =>
                (i.GetType().GenericTypeArguments[0].Name == typeName) &&
                i.FileExtension == ext);

            if (descriptor == null)
            {
                return Result.Fail($"Импортер для {typeName} и расширения {ext} не найден.");
            }

            try
            {
                await descriptor.ImportAsync(filePath);
                return Result.Ok();
            }
            catch (ImportException ex)
            {
                return Result.Fail($"Ошибка импорта: {ex.Message}");
            }
            catch (RepositoryException ex)
            {
                return Result.Fail($"Ошибка репозитория: {ex.Message}");
            }
        }
    }
}