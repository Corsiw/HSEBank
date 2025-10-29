namespace Infrastructure.Repositories
{
    public class InMemoryRepository<T>(Func<T, Guid> idSelector) : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _items = new Dictionary<Guid, T>();

        public Task AddAsync(T item)
        {
            _items[idSelector(item)] = item;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T item)
        {
            _items[idSelector(item)] = item;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _items.Remove(id);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync(Guid id)
        {
            return Task.FromResult(_items.GetValueOrDefault(id));
        }

        public Task<IEnumerable<T>> ListAsync()
        {
            return Task.FromResult(_items.Values.AsEnumerable());
        }
    }
}