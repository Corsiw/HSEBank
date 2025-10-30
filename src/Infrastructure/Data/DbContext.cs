using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
    {
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Operation> Operations => Set<Operation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка ключей
            modelBuilder.Entity<BankAccount>().HasKey(b => b.Id);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Operation>().HasKey(o => o.Id);

            // Настройка связей
            modelBuilder.Entity<Operation>()
                .HasOne<BankAccount>()
                .WithMany()
                .HasForeignKey(o => o.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Operation>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(o => o.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}