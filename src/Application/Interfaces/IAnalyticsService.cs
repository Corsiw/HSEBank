using Domain.Analytics;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<IAnalyticsResult> AnalyzeAsync(string strategyName);
        IReadOnlyCollection<string> GetAvailableStrategies();
    }
}