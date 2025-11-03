using Application.Analytics;

namespace Tests
{
    public class TestAnalyticsVisitor : IAnalyticsResultVisitor
    {
        public List<string> Output { get; } = new();

        public void Visit<T>(AnalyticsResult<T> result)
        {
            foreach (T item in result.Data)
            {
                Output.Add(item?.ToString() ?? "<null>");
            }
        }
    }
}