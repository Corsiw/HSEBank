namespace Domain.Exceptions
{
    public class ExportException : Exception
    {
        public ExportException(string message) : base(message) { }

        public ExportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}