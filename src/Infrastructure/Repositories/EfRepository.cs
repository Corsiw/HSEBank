using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await _dbSet.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T item)
        {
            PropertyInfo prop = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException("Entity must have Id property");
            Guid id = (Guid)prop.GetValue(item)!;
            T? existing = await GetAsync(id);
            if (existing == null)
            {
                await AddAsync(item);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            T? entity = await GetAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}