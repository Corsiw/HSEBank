using Application.Interfaces;
using Domain.Common;
using Domain.Interfaces;
using Infrastructure.Export;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class ExportService(
        IEnumerable<IRepository<IVisitable>> repos,
        IEnumerable<ExporterDescriptor> exporters)
        : IExportService
    {
        public async Task<Result> ExportFileByTypeAsync(string filePath, Type type)
        {
            string ext = Path.GetExtension(filePath).ToLower();

            // Находим экспортер по расширению и поддерживаемому типу
            ExporterDescriptor? descriptor = exporters.FirstOrDefault(e =>
                e.Exporter.FileExtension.Equals(ext, StringComparison.OrdinalIgnoreCase) &&
                e.SupportedTypes.Contains(type));

            if (descriptor == null)
            {
                return Result.Fail($"Экспортер для {type.Name} и расширения {ext} не найден");
            }

            try
            {
                IRepository<IVisitable>? repo = repos.FirstOrDefault(r =>
                    r.ListAsync().Result.FirstOrDefault()?.GetType() == type);

                if (repo == null)
                {
                    return Result.Fail($"Репозиторий для типа {type.Name} не найден");
                }

                IEnumerable<IVisitable> items = await repo.ListAsync();

                descriptor.Exporter.BeginExport();
                foreach (IVisitable item in items)
                {
                    descriptor.Exporter.ExportItem(item);
                }

                await descriptor.Exporter.EndExport(filePath);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        
        public IReadOnlyCollection<(Type type, string extension)> GetTypeExtensionOptions()
        {
            return exporters
                .SelectMany(e => e.SupportedTypes.Select(t => (t, e.Exporter.FileExtension)))
                .Distinct()
                .ToList()
                .AsReadOnly();
        }
    }
}
