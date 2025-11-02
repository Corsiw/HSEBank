using Application.Analytics;

namespace Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<IAnalyticsResult> AnalyzeAsync(string strategyName);
        IReadOnlyCollection<string> GetAvailableStrategies();
    }
}