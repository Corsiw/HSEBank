using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class RepositoryAdapter<T>(IRepository<T> inner) : IRepository<IVisitable>
        where T : class, IVisitable
    {
        public async Task AddAsync(IVisitable item)
        {
            await inner.AddAsync((T)item);
        }

        public async Task AddRangeAsync(IEnumerable<IVisitable> items)
        {
            await inner.AddRangeAsync((IEnumerable<T>)items);
        }

        public async Task UpdateAsync(IVisitable item)
        {
            await inner.UpdateAsync((T)item);
        }

        public Task UpsertAsync(IVisitable item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id)
        {
            await inner.DeleteAsync(id);
        }

        public async Task<IVisitable?> GetAsync(Guid id)
        {
            return await inner.GetAsync(id);
        }

        public async Task<IEnumerable<IVisitable>> ListAsync()
        {
            return await inner.ListAsync();
        }
    }
}