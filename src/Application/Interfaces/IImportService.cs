using Domain.Common;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Task<Result> ImportFileByTypeAsync(string filePath, string type);
        IReadOnlyCollection<(string type, string extension)> GetTypeExtensionOptions();
    }
}