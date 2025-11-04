using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    // For correct design time migrations 
    public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
    {
        public FinanceDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new();
            optionsBuilder.UseSqlite(DbPathProvider.GetConnectionString());

            return new FinanceDbContext(optionsBuilder.Options);
        }
    }
}