using Domain.Common;

namespace Application.Interfaces
{
    public interface IExportService
    {
        Task<Result> ExportFileByTypeAsync(string filePath, Type type);

        IReadOnlyCollection<(Type type, string extension)> GetTypeExtensionOptions();
    }
}