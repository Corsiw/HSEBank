namespace Infrastructure.Repositories
{
    public class CachedRepositoryProxy<T>(IRepository<T> inner, Func<T, Guid> idSelector) : IRepository<T>
        where T : class
    {
        private readonly Dictionary<Guid, T> _cache = new();

        public async Task AddAsync(T item)
        {
            await inner.AddAsync(item);
            _cache[idSelector(item)] = item;
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            IEnumerable<T> enumerable = items.ToList();
            await inner.AddRangeAsync(enumerable);
            foreach (T item in enumerable)
            {
                _cache[idSelector(item)] = item;
            }
        }

        public async Task UpdateAsync(T item)
        {
            await inner.UpdateAsync(item);
            _cache[idSelector(item)] = item;
        }

        public async Task DeleteAsync(Guid id)
        {
            await inner.DeleteAsync(id);
            _cache.Remove(id);
        }

        public async Task<T?> GetAsync(Guid id)
        {
            if (_cache.TryGetValue(id, out T? v))
            {
                return v;
            }

            T? val = await inner.GetAsync(id);
            if (val != null)
            {
                _cache[id] = val;
            }

            return val;
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            IEnumerable<T> items = await inner.ListAsync();
            foreach (T i in items)
            {
                _cache[idSelector(i)] = i;
            }

            return _cache.Values.ToList();
        }
    }
}