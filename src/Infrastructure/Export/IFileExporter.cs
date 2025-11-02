using Domain.Interfaces;

namespace Infrastructure.Export
{
    public interface IFileExporter
    {
        string FileExtension { get; }
        void BeginExport();
        void ExportItem(IVisitable item);
        Task EndExport(string filePath);
    }
}