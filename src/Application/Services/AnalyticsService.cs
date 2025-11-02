using Application.Interfaces;
using Application.Strategies;
using Domain.Analytics;

namespace Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly Dictionary<string, IAnalyticsStrategy> _strategies;

        public AnalyticsService(IEnumerable<IAnalyticsStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.Name, s => s);
        }

        public async Task<IAnalyticsResult> AnalyzeAsync(string strategyName)
        {
            if (!_strategies.TryGetValue(strategyName, out IAnalyticsStrategy? strategy))
            {
                throw new ArgumentException($"Strategy '{strategyName}' not found.");
            }

            return await strategy.AnalyzeAsync();
        }

        public IReadOnlyCollection<string> GetAvailableStrategies()
        {
            return _strategies.Keys;
        }
    }
}