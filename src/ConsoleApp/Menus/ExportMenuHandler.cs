using Application.Interfaces;
using Domain.Common;

namespace ConsoleApp.Menus
{
    public class ExportMenuHandler(IExportService exportService) : IMenuHandler
    {
        public string Name => "Экспорт";

        public async Task HandleAsync()
        {
            while (true)
            {
                List<Type> types = exportService.GetTypeExtensionOptions()
                    .Select(s => s.type)
                    .Distinct()
                    .ToList();
                types.Sort((t1, t2)  => string.Compare(t1.Name, t2.Name, StringComparison.Ordinal));

                List<string> extensions = exportService.GetTypeExtensionOptions()
                    .Select(s => s.extension)
                    .Distinct()
                    .ToList();

                Console.WriteLine($"{Environment.NewLine}=== Экспорт данных ===");
                Console.WriteLine("Доступные типы данных:");

                int i = 1;
                foreach (Type type in types)
                {
                    Console.WriteLine($"{i++}. {type.Name}");
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

                Type selectedType = types.ElementAt(typeIndex - 1);

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

                string filePath = InputHelper.ReadString($"Введите путь для сохранения файла ({selectedExt}): ");


                Result result = await exportService.ExportFileByTypeAsync(filePath, selectedType);

                Console.WriteLine(result.IsSuccess
                    ? $"Экспорт {selectedType} успешно завершён. Файл: {filePath}"
                    : $"Ошибка: {result.Message}");
            }
        }
    }
}
