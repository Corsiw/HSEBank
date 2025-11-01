using Domain.Exceptions;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Декоратор для репозитория, обеспечивающий безопасное выполнение операций.
    /// Все исключения инфраструктурного уровня перехватываются и оборачиваются в RepositoryException.
    /// </summary>
    public class SafeRepositoryDecorator<T>(IRepository<T> inner) : IRepository<T>
        where T : class
    {
        public async Task AddAsync(T entity)
        {
            await ExecuteSafeAsync(() => inner.AddAsync(entity));
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await ExecuteSafeAsync(() => inner.AddRangeAsync(entities));
        }

        public async Task UpdateAsync(T entity)
        {
            await ExecuteSafeAsync(() => inner.UpdateAsync(entity));
        }

        public async Task UpsertAsync(T item)
        {
            await ExecuteSafeAsync(() => inner.UpsertAsync(item));
        }

        public async Task DeleteAsync(Guid id)
        {
            await ExecuteSafeAsync(() => inner.DeleteAsync(id));
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => inner.GetAsync(id));
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            return await ExecuteSafeAsync(inner.ListAsync);
        }

        private static async Task ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                throw RepositoryException.Wrap(ex);
            }
        }

        private static async Task<TResult> ExecuteSafeAsync<TResult>(Func<Task<TResult>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                throw RepositoryException.Wrap(ex);
            }
        }
    }
}
