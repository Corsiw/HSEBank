using Domain.Common;

namespace Infrastructure.Import
{
    public interface IFileImporter
    {
        Task ImportAsync(string filePath);
    }

    public interface IFileImporter<T> : IFileImporter where T : class
    {
        new Task ImportAsync(string filePath);
    }
}