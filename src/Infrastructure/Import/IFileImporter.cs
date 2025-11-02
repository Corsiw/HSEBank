namespace Infrastructure.Import
{
    public interface IFileImporter
    {
        string FileExtension { get; }
        Task ImportAsync(string filePath);
    }

    public interface IFileImporter<T> : IFileImporter where T : class
    {
        new Task ImportAsync(string filePath);
    }
}