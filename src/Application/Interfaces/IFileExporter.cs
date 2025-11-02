using Domain.Interfaces;

namespace Application.Interfaces
{
    public interface IFileExporter
    {
        string FileExtension { get; }
        void BeginExport();
        void ExportItem(IVisitable item);
        Task EndExport(string filePath);
    }
}