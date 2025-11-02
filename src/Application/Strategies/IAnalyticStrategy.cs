using Domain.Analytics;

namespace Application.Strategies
{
    public interface IAnalyticsStrategy
    {
        string Name { get; }
        Task<IAnalyticsResult> AnalyzeAsync();
    }

}