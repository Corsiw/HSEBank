using Application.Interfaces;
using Application.Services;
using ConsoleApp.Menus;
using Domain.Attributes;
using Domain.Entities;
using Domain.Factories;
using Infrastructure.Data;
using Infrastructure.Import;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ConsoleApp
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ServiceCollection services = new();

            // Domain
            services.AddScoped<IDomainFactory, DomainFactory>();

            // Infrastructure
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlite("Data Source=finance.db"));

            services.AddScoped<IRepository<BankAccount>>(sp =>
            {
                FinanceDbContext dbContext = sp.GetRequiredService<FinanceDbContext>();
                EfRepository<BankAccount> efRepo = new EfRepository<BankAccount>(dbContext);
                return new CachedRepositoryProxy<BankAccount>(efRepo, b => b.Id);
            });

            services.AddScoped<IRepository<Category>>(sp =>
            {
                FinanceDbContext dbContext = sp.GetRequiredService<FinanceDbContext>();
                EfRepository<Category> efRepo = new EfRepository<Category>(dbContext);
                return new CachedRepositoryProxy<Category>(efRepo, c => c.Id);
            });

            services.AddScoped<IRepository<Operation>>(sp =>
            {
                FinanceDbContext dbContext = sp.GetRequiredService<FinanceDbContext>();
                EfRepository<Operation> efRepo = new EfRepository<Operation>(dbContext);
                return new CachedRepositoryProxy<Operation>(efRepo, o => o.Id);
            });
            
            // Importers
            IEnumerable<Type> domainTypes = Assembly.GetAssembly(typeof(ImportedAttribute))
                !.GetTypes()
                .Where(t => t is { IsAbstract: false, BaseType.IsGenericType: false }
                            && t.GetCustomAttribute<ImportedAttribute>() != null).ToArray();
            
            IEnumerable<Type> importerBaseTypes = Assembly.GetAssembly(typeof(ImporterAttribute))
                !.GetTypes()
                .Where(t => t is { IsAbstract: false, BaseType.IsGenericType: true }
                            && t.BaseType.GetGenericTypeDefinition() == typeof(FileImporterBase<>)
                            && t.GetCustomAttribute<ImporterAttribute>() != null).ToArray();

            foreach (Type domainType in domainTypes)
            {
                foreach (Type importerBaseType in importerBaseTypes)
                {
                    Type closedType = importerBaseType.MakeGenericType(domainType);
                    services.AddScoped(closedType);
                }
            }
            
            services.AddScoped<IImportService>(sp =>
            {
                List<object> importers = [];
                foreach (Type domainType in domainTypes)
                {
                    foreach (Type importerBaseType in importerBaseTypes)
                    {
                        Type closedType = importerBaseType.MakeGenericType(domainType);
                        importers.Add(sp.GetRequiredService(closedType));
                    }
                }
                return new ImportService(importers);
            });

            // Application
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOperationService, OperationService>();

            // ConsoleApp
            services.AddSingleton<AccountMenuHandler>();
            services.AddSingleton<CategoryMenuHandler>();
            services.AddSingleton<OperationMenuHandler>();
            services.AddSingleton<ImportMenuHandler>();
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<AccountMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<CategoryMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<OperationMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<ImportMenuHandler>());
            services.AddSingleton<ConsoleApp>();

            ServiceProvider provider = services.BuildServiceProvider();

            using IServiceScope scope = provider.CreateScope();
            
            FinanceDbContext db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
            Console.WriteLine("Checking database migrations...");
            await db.Database.MigrateAsync();
            Console.WriteLine("Database is up to date.");
                
            ConsoleApp app = scope.ServiceProvider.GetRequiredService<ConsoleApp>();
            await app.RunAsync();
        }
    }
}