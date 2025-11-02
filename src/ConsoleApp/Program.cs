using Application.Descriptors;
using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Application.Strategies;
using ConsoleApp.Menus;
using Domain.Common;
using Domain.Entities;
using Domain.Factories;
using Domain.Interfaces;
using Infrastructure.Analytics;
using Infrastructure.Data;
using Infrastructure.Export;
using Infrastructure.Import;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
                EfRepository<BankAccount> efRepo = new(dbContext);
                CachedRepositoryProxy<BankAccount> cachedRepo =  new(efRepo, b => b.Id);
                return new SafeRepositoryDecorator<BankAccount>(cachedRepo);
            });

            services.AddScoped<IRepository<Category>>(sp =>
            {
                FinanceDbContext dbContext = sp.GetRequiredService<FinanceDbContext>();
                EfRepository<Category> efRepo = new EfRepository<Category>(dbContext);
                CachedRepositoryProxy<Category> cachedRepo =  new(efRepo, b => b.Id);
                return new SafeRepositoryDecorator<Category>(cachedRepo);
            });

            services.AddScoped<IRepository<Operation>>(sp =>
            {
                FinanceDbContext dbContext = sp.GetRequiredService<FinanceDbContext>();
                EfRepository<Operation> efRepo = new EfRepository<Operation>(dbContext);
                CachedRepositoryProxy<Operation> cachedRepo =  new(efRepo, b => b.Id);
                return new SafeRepositoryDecorator<Operation>(cachedRepo);
            });
            
            // Importers
            services.AddScoped<IImportProfile<BankAccount, BankAccountDto>, BankAccountImportProfile>();
            services.AddScoped<IImportProfile<Category, CategoryDto>, CategoryImportProfile>();
            services.AddScoped<IImportProfile<Operation, OperationDto>, OperationImportProfile>();

            // CSV
            services.AddScoped<IFileImporter, CsvImporter<BankAccount, BankAccountDto>>();
            services.AddScoped<IFileImporter, CsvImporter<Category, CategoryDto>>();
            services.AddScoped<IFileImporter, CsvImporter<Operation, OperationDto>>();

            // JSON
            services.AddScoped<IFileImporter, JsonImporter<BankAccount, BankAccountDto>>();
            services.AddScoped<IFileImporter, JsonImporter<Category, CategoryDto>>();
            services.AddScoped<IFileImporter, JsonImporter<Operation, OperationDto>>();
            services.AddScoped<IImportService, ImportService>(p =>
            {
                List<IFileImporter> importers = p.GetServices<IFileImporter>().ToList();
                return new ImportService(importers);
            });
            
            // Exporters
            services.AddScoped<IRepository<IVisitable>>(sp =>
                new RepositoryAdapter<BankAccount>(sp.GetRequiredService<IRepository<BankAccount>>()));
            
            services.AddScoped<IRepository<IVisitable>>(sp =>
                new RepositoryAdapter<Category>(sp.GetRequiredService<IRepository<Category>>()));
            
            services.AddScoped<IRepository<IVisitable>>(sp =>
                new RepositoryAdapter<Operation>(sp.GetRequiredService<IRepository<Operation>>()));
            
            services.AddScoped<IFileExporter, JsonFileVisitor>();
            services.AddScoped<IFileExporter, CsvFileVisitor>();
            
            services.AddScoped<IEnumerable<ExporterDescriptor>>(provider =>
            {
                List<IFileExporter> exporters = provider.GetServices<IFileExporter>().ToList();

                CsvFileVisitor csv = exporters.OfType<CsvFileVisitor>().First();
                JsonFileVisitor json = exporters.OfType<JsonFileVisitor>().First();

                return new List<ExporterDescriptor>
                {
                    new(csv, [typeof(BankAccount), typeof(Operation), typeof(Category)]),
                    new(json, [typeof(BankAccount), typeof(Operation), typeof(Category)])
                };
            });
            
            services.AddScoped<IExportService, ExportService>();
            
            // Analytics
            services.AddScoped<IAnalyticsStrategy, ExpenseByCategoryStrategy>();
            services.AddScoped<IAnalyticsStrategy, BalanceDynamicsStrategy>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();

            // Application
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOperationService, OperationService>();

            // ConsoleApp
            services.AddSingleton<AccountMenuHandler>();
            services.AddSingleton<CategoryMenuHandler>();
            services.AddSingleton<OperationMenuHandler>();
            services.AddSingleton<ImportMenuHandler>();
            services.AddSingleton<ExportMenuHandler>();
            services.AddSingleton<AnalyticsMenuHandler>();
            services.AddSingleton<IAnalyticsResultVisitor, ConsoleAnalyticsResultVisitor>();
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<AccountMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<CategoryMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<OperationMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<ImportMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<ExportMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<AnalyticsMenuHandler>());
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