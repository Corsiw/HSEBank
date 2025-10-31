using Application.Interfaces;
using Domain.Common;

namespace ConsoleApp.Menus
{
    public class ImportMenuHandler(IImportService importService) : IMenuHandler
    {
        public string Name => "Импорт";

        public async Task HandleAsync()
        {
            while (true)
            {
                List<string> types = importService.GetTypeExtensionOptions().Select(s => s.type).Distinct().ToList();
                List<string> extensions = importService.GetTypeExtensionOptions().Select(s => s.extension).Distinct().ToList();

                Console.WriteLine($"{Environment.NewLine}=== Импорт данных ===");
                Console.WriteLine("Доступные типы данных:");

                int i = 1;
                foreach (string type in types)
                {
                    Console.WriteLine($"{i++}. {type}");
                }

                Console.WriteLine($"{types.Count + 1}. Выйти");

                int typeIndex = InputHelper.ReadInt("Выберите тип: ");
                if (typeIndex <= 0 || typeIndex > types.Count + 1)
                {
                    Console.WriteLine("Некорректный выбор.");
                    continue;
                }

                if (typeIndex == types.Count + 1)
                {
                    break;
                }

                string selectedType = types.ElementAt(typeIndex - 1);

                Console.WriteLine($"{Environment.NewLine}Доступные расширения:");
                i = 1;
                foreach (string ext in extensions)
                {
                    Console.WriteLine($"{i++}. {ext}");
                }

                Console.WriteLine($"{extensions.Count + 1}. Назад");

                int extIndex = InputHelper.ReadInt("Выберите расширение: ");
                if (extIndex <= 0 || extIndex > extensions.Count + 1)
                {
                    Console.WriteLine("Некорректный выбор.");
                    continue;
                }

                if (extIndex == extensions.Count + 1)
                {
                    continue;
                }

                string selectedExt = extensions.ElementAt(extIndex - 1);

                string filePath = InputHelper.ReadString($"Введите путь к файлу ({selectedExt}): ");
                Result result = await importService.ImportFileByTypeAsync(filePath, selectedType);

                Console.WriteLine(result.IsSuccess
                    ? $"Импорт {selectedType} успешно завершён."
                    : $"Ошибка: {result.Message}");
            }
        }
    }
}
