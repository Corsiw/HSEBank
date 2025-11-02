using Application.Analytics;

namespace Infrastructure.Analytics
{
    public class ConsoleAnalyticsResultVisitor : IAnalyticsResultVisitor
    {
        public void Visit<T>(AnalyticsResult<T> result)
        {
            Console.WriteLine($"=== Результаты анализа ({typeof(T).Name}) ===");
            foreach (T item in result.Data)
            {
                Console.WriteLine(item);
            }
        }
    }

}