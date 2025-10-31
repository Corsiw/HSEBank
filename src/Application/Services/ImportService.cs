using Application.Interfaces;
using Domain.Common;
using Domain.Exceptions;
using Infrastructure.Import;
using System.Reflection;

namespace Application.Services
{
    public class ImportService : IImportService
    {
        private readonly Dictionary<(string type, string extension), IFileImporter> _importers = new();

        public ImportService(IEnumerable<object> importers)
        {
            foreach (IFileImporter importer in importers)
            {
                if (importer.GetType().GetCustomAttribute<ImporterAttribute>() is not { } attr)
                {
                    continue;
                }

                string type = importer.GetType().GenericTypeArguments[0].Name;
                _importers[(type, attr.FileExtension)] = importer;
            }
        }

        public IReadOnlyCollection<(string type, string extension)> GetTypeExtensionOptions()
        {
            return _importers.Keys;
        }

        public async Task<Result> ImportFileByTypeAsync(string filePath, string typeName)
        {
            filePath = filePath.Trim();
            if (!File.Exists(filePath))
            {
                return Result.Fail($"Файл {filePath} не найден.");
            }
            
            string domainType = _importers.Keys.FirstOrDefault(k => k.type == typeName).type;
            if (domainType == null)
            {
                return Result.Fail($"Тип {typeName} не найден.");
            }
            
            string ext = Path.GetExtension(filePath).ToLower();

            if (!_importers.TryGetValue((domainType, ext), out IFileImporter? importer))
            {
                return Result.Fail($"Импортер для {domainType} с расширением {ext} не найден.");
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
        }
    }
}