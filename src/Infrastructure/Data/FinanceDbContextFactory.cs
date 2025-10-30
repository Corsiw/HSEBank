using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
    {
        public FinanceDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<FinanceDbContext> optionsBuilder = new();
            optionsBuilder.UseSqlite("Data Source=finance.db");

            return new FinanceDbContext(optionsBuilder.Options);
        }
    }
}