using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace ConsoleApp.Menus
{
    public class OperationMenuHandler(IOperationService service) : IMenuHandler
    {
        public string Name => "Операции";

        public async Task HandleAsync()
        {
            string? cmd;
            do
            {
                Console.WriteLine($"{Environment.NewLine}=== Управление операциями ===");
                Console.WriteLine("1. Добавить операцию");
                Console.WriteLine("2. Изменить описание");
                Console.WriteLine("3. Удалить операцию");
                Console.WriteLine("4. Показать все операции");
                Console.WriteLine("5. Назад");
                cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1": await AddOperationAsync(); break;
                    case "2": await UpdateDescriptionAsync(); break;
                    case "3": await DeleteAsync(); break;
                    case "4": await ShowAllAsync(); break;
                }
            } while (cmd != "5");
        }

        private async Task AddOperationAsync()
        {
            Guid accountId = InputHelper.ReadGuid("Введите ID счета: ");
            Guid categoryId = InputHelper.ReadGuid("Введите ID категории: ");
            MoneyType type = InputHelper.ReadEnum<MoneyType>("Тип операции (0 - Доход, 1 - Расход): ");
            decimal amount = InputHelper.ReadDecimal("Сумма: ");
            string description = InputHelper.ReadString("Описание (необязательно): ");
            Result<Operation> result = await service.AddOperationAsync(type, accountId, categoryId, amount, DateTime.Now, description);
            Console.WriteLine(result.IsSuccess ? "Операция добавлена" : $"{result.Message}");
        }

        private async Task UpdateDescriptionAsync()
        {
            Guid id = InputHelper.ReadGuid("Введите ID операции: ");
            string description = InputHelper.ReadString("Новое описание: ");
            Result<Operation> result = await service.UpdateDescriptionAsync(id, description);
            Console.WriteLine(result.IsSuccess ? "Обновлено" : $"{result.Message}");
        }

        private async Task DeleteAsync()
        {
            Guid id = InputHelper.ReadGuid("Введите ID операции: ");
            Result result = await service.DeleteOperationAsync(id);
            Console.WriteLine(result.IsSuccess ? "Удалено" : $"{result.Message}");
        }

        private async Task ShowAllAsync()
        {
            IEnumerable<Operation> ops = await service.GetAllAsync();
            Console.WriteLine($"{Environment.NewLine}Операции:");
            foreach (Operation op in ops)
            {
                Console.WriteLine($"- {op.Type} {op.Amount} ({op.Description}) — {op.Date:d}, Account={op.BankAccountId}, Category={op.CategoryId}");
            }
        }
    }
}
