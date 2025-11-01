namespace Infrastructure.Export
{
    public class ExporterDescriptor(IFileExporter exporter, IEnumerable<Type> supportedTypes)
    {
        public IFileExporter Exporter { get; } = exporter ?? throw new ArgumentNullException(nameof(exporter));
        public IReadOnlyCollection<Type> SupportedTypes { get; } = supportedTypes.ToList().AsReadOnly()
                                                                   ?? throw new ArgumentNullException(nameof(supportedTypes));
    }
}