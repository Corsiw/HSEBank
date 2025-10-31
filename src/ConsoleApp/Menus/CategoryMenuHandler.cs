using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace ConsoleApp.Menus
{
    public class CategoryMenuHandler(ICategoryService service) : IMenuHandler
    {
        public string Name => "Категории";

        public async Task HandleAsync()
        {
            string? cmd;
            do
            {
                Console.WriteLine($"{Environment.NewLine}=== Управление категориями ===");
                Console.WriteLine("1. Создать категорию");
                Console.WriteLine("2. Переименовать категорию");
                Console.WriteLine("3. Удалить категорию");
                Console.WriteLine("4. Показать все категории");
                Console.WriteLine("5. Назад");
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
                        await DeleteAsync();
                        break;
                    case "4":
                        await ShowAllAsync();
                        break;
                }
            } while (cmd != "5");
        }

        private async Task CreateAsync()
        {
            MoneyType type = InputHelper.ReadEnum<MoneyType>("Тип (0 - Доход, 1 - Расход): ");
            string name = InputHelper.ReadString("Название: ");
            Result<Category> create = await service.CreateCategoryAsync(type, name);
            Console.WriteLine(create.IsSuccess ? $"{create.Value!.Name} создана." : $"{create.Message}");
        }

        private async Task RenameAsync()
        {
            await ShowAllAsync();
            Guid id = InputHelper.ReadGuid("Введите ID категории: ");
            string name = InputHelper.ReadString("Новое имя: ");
            Result<Category> result = await service.UpdateNameAsync(id, name);
            Console.WriteLine(result.IsSuccess ? "Обновлено" : $"{result.Message}");
        }

        private async Task DeleteAsync()
        {
            await ShowAllAsync();
            Guid id = InputHelper.ReadGuid("Введите ID категории: ");
            Result result = await service.DeleteCategoryAsync(id);
            Console.WriteLine(result.IsSuccess ? "Удалено" : $"{result.Message}");
        }

        private async Task ShowAllAsync()
        {
            IEnumerable<Category> categories = await service.GetAllAsync();
            Console.WriteLine($"{Environment.NewLine}Категории:");
            foreach (Category c in categories)
            {
                Console.WriteLine($"- {c.Name} ({c.Type}), Id={c.Id}");
            }
        }
    }
}
