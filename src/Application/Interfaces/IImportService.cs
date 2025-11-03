using Domain.Common;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Task<Result> ImportFileByTypeAsync(string filePath, Type type);
        IReadOnlyCollection<(Type type, string extension)> GetTypeExtensionOptions();
    }
}