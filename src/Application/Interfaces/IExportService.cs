using Domain.Common;

namespace Application.Interfaces
{
    public interface IExportService
    {
        Task<Result> ExportFileByTypeAsync(string filePath, Type typeName);

        IReadOnlyCollection<(Type type, string extension)> GetTypeExtensionOptions();
    }
}