using Application.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace ConsoleApp.Menus
{
    public class AccountMenuHandler(IBankAccountService service) : IMenuHandler
    {
        public string Name => "Счета";

        public async Task HandleAsync()
        {
            string? cmd;
            do
            {
                Console.WriteLine($"{Environment.NewLine}=== Управление счетами ===");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Переименовать счет");
                Console.WriteLine("3. Изменить баланс");
                Console.WriteLine("4. Удалить счет");
                Console.WriteLine("5. Показать все счета");
                Console.WriteLine("6. Назад");
                cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1":
                        await CreateAsync();
                        break;
                    case "2":
                        await RenameAsync();
                        break;
                    case "3":
                        await UpdateBalanceAsync();
                        break;
                    case "4":
                        await DeleteAsync();
                        break;
                    case "5":
                        await ShowAllAsync();
                        break;
                }
            } while (cmd != "6");
        }

        private async Task CreateAsync()
        {
            string name = InputHelper.ReadString("Название счета: ");
            decimal balance = InputHelper.ReadDecimal("Начальный баланс: ");
            Result<BankAccount> create = await service.CreateAccountAsync(name, balance);
            Console.WriteLine(create.IsSuccess ? $"{create.Value!.Name} создан." : $"{create.Message}");
        }

        private async Task RenameAsync()
        {
            await ShowAllAsync();
            Guid id = InputHelper.ReadGuid("Введите ID счета: ");
            string newName = InputHelper.ReadString("Новое имя: ");
            Result<BankAccount> result = await service.UpdateNameAsync(id, newName);
            Console.WriteLine(result.IsSuccess ? "Обновлено" : $"{result.Message}");
        }

        private async Task UpdateBalanceAsync()
        {
            await ShowAllAsync();
            Guid id = InputHelper.ReadGuid("Введите ID счета: ");
            decimal newBalance = InputHelper.ReadDecimal("Новый баланс: ");
            Result<BankAccount> result = await service.UpdateBalanceAsync(id, newBalance);
            Console.WriteLine(result.IsSuccess ? "Баланс обновлен" : $"{result.Message}");
        }

        private async Task DeleteAsync()
        {
            await ShowAllAsync();
            Guid id = InputHelper.ReadGuid("Введите ID счета: ");
            Result result = await service.DeleteAccountAsync(id);
            Console.WriteLine(result.IsSuccess ? "Удалено" : $"{result.Message}");
        }

        private async Task ShowAllAsync()
        {
            IEnumerable<BankAccount> accounts = await service.GetAllAsync();
            Console.WriteLine($"{Environment.NewLine}Счета:");
            foreach (BankAccount a in accounts)
            {
                Console.WriteLine($"- {a.Name} ({a.Id}), баланс: {a.Balance}");
            }
        }
    }
}