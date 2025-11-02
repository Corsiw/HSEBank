using Application.Interfaces;
using Domain.Analytics;
using Domain.Entities;

namespace Application.Strategies
{
    public class BalanceDynamicsStrategy(IRepository<Operation> operationRepo) : IAnalyticsStrategy
    {
        public string Name => "BalanceDynamics";

        public async Task<IAnalyticsResult> AnalyzeAsync()
        {
            IEnumerable<Operation> operations = await operationRepo.ListAsync();
            List<DateBalance> byDate = operations
                .GroupBy(o => o.Date.Date)
                .Select(g => new DateBalance(g.Key, g.Sum(o => o.Amount)))
                .ToList();

            return new AnalyticsResult<DateBalance>(byDate);
        }
    }
}