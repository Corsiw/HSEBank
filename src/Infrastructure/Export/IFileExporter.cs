using Domain.Interfaces;

namespace Infrastructure.Export
{
    public interface IFileExporter
    {
        public string FileExtension { get; }
        void BeginExport();
        void ExportItem(IVisitable item);
        Task EndExport(string filePath);
    }
}