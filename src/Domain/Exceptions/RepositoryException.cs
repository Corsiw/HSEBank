namespace Domain.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static RepositoryException Wrap(Exception inner)
        {
            return new RepositoryException($"Ошибка работы с хранилищем данных: {inner.Message}", inner);
        }
    }
}