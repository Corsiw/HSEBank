namespace Infrastructure.Import
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImporterAttribute(string fileExtension) : Attribute
    {
        public string FileExtension { get; } = fileExtension;
    }
}