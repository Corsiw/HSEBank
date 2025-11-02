using Application.Analytics;
using Application.Interfaces;

namespace ConsoleApp.Menus
{
    public class AnalyticsMenuHandler(IAnalyticsService analyticsService, IAnalyticsResultVisitor resultVisitor) : IMenuHandler
    {
        public string Name => "Аналитика";

        public async Task HandleAsync()
        {
            while (true)
            {
                Console.WriteLine($"{Environment.NewLine}=== Аналитика ===");
                IReadOnlyCollection<string> strategies = analyticsService.GetAvailableStrategies();

                if (strategies.Count == 0)
                {
                    Console.WriteLine("Нет доступных стратегий анализа.");
                    return;
                }

                Console.WriteLine("Доступные стратегии анализа:");
                int i = 1;
                foreach (string strategy in strategies)
                {
                    Console.WriteLine($"{i++}. {strategy}");
                }

                Console.WriteLine($"{strategies.Count + 1}. Выйти");

                int choice = InputHelper.ReadInt("Выберите стратегию: ");
                if (choice == strategies.Count + 1)
                {
                    break;
                }

                if (choice <= 0 || choice > strategies.Count)
                {
                    Console.WriteLine("Некорректный выбор.");
                    continue;
                }

                string selectedStrategy = strategies.ElementAt(choice - 1);
                Console.WriteLine($"Запуск анализа: {selectedStrategy}...");

                IAnalyticsResult result = await analyticsService.AnalyzeAsync(selectedStrategy);
                result.Accept(resultVisitor);
            }
        }
    }
}
