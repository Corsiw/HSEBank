namespace Infrastructure.Repositories
{
    public interface IRepository<T>
    {
        Task AddAsync(T item);
        Task AddRangeAsync(IEnumerable<T> items);
        Task UpdateAsync(T item);
        Task UpsertAsync(T item);
        Task DeleteAsync(Guid id);
        Task<T?> GetAsync(Guid id);
        Task<IEnumerable<T>> ListAsync();
    }
}