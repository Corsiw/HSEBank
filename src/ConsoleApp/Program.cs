using Application.Interfaces;
using Application.Services;
using ConsoleApp.Menus;
using Domain.Entities;
using Domain.Factories;
using Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IDomainFactory, DomainFactory>();
            
            // Application
            services.AddSingleton<IRepository<BankAccount>>(_ =>
                new CachedRepositoryProxy<BankAccount>(
                    new InMemoryRepository<BankAccount>(a => a.Id),
                    a => a.Id));
            
            services.AddSingleton<IRepository<Category>>(_ =>
                new CachedRepositoryProxy<Category>(
                    new InMemoryRepository<Category>(c => c.Id),
                    c => c.Id));

            services.AddSingleton<IRepository<Operation>>(_ =>
                new CachedRepositoryProxy<Operation>(
                    new InMemoryRepository<Operation>(o => o.Id),
                    o => o.Id));

            services.AddSingleton<IBankAccountService, BankAccountService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IOperationService, OperationService>();

            // ConsoleApp
            services.AddSingleton<AccountMenuHandler>();
            services.AddSingleton<CategoryMenuHandler>();
            services.AddSingleton<OperationMenuHandler>();
            
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<AccountMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<CategoryMenuHandler>());
            services.AddSingleton<IMenuHandler>(sp => sp.GetRequiredService<OperationMenuHandler>());

            services.AddTransient<ConsoleApp>();
            
            ServiceProvider provider = services.BuildServiceProvider();
            ConsoleApp app = provider.GetRequiredService<ConsoleApp>();

            await app.RunAsync();
        }
    }
}