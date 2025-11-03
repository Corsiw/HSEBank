namespace Application.Interfaces
{
    public interface IFileImporter
    {
        string FileExtension { get; }
        Task ImportAsync(string filePath);
    }
}