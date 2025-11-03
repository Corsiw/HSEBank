using Application.Interfaces;
using Domain.Common;
using Domain.Exceptions;

namespace Application.Services
{
    public class ImportService(IEnumerable<IFileImporter> importers) : IImportService
    {
        private readonly IReadOnlyCollection<IFileImporter> _importers = importers.ToList().AsReadOnly();

        public IReadOnlyCollection<(Type type, string extension)> GetTypeExtensionOptions()
        {
            return _importers
                .Select(t => (t.GetType().GenericTypeArguments[0], t.FileExtension))
                .Distinct()
                .ToList()
                .AsReadOnly();
        }

        public async Task<Result> ImportFileByTypeAsync(string filePath, Type type)
        {
            filePath = filePath.Trim();
            if (!File.Exists(filePath))
            {
                return Result.Fail($"Файл {filePath} не найден.");
            }
            
            string ext = Path.GetExtension(filePath).ToLower();

            IFileImporter? importer = _importers.FirstOrDefault(i =>
                (i.GetType().GenericTypeArguments[0] == type) &&
                i.FileExtension == ext);

            if (importer == null)
            {
                return Result.Fail($"Импортер для {type} и расширения {ext} не найден.");
            }

            try
            {
                await importer.ImportAsync(filePath);
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