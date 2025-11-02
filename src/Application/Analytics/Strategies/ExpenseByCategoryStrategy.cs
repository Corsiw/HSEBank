using Application.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace Application.Analytics.Strategies
{
    public class ExpenseByCategoryStrategy(IRepository<Operation> operationRepo) : IAnalyticsStrategy
    {
        public string Name => "ExpensesByCategory";

        public async Task<IAnalyticsResult> AnalyzeAsync()
        {
            IEnumerable<CategoryExpense> grouped = (await operationRepo.ListAsync())
                .GroupBy(o => o.CategoryId)
                .Select(g => new CategoryExpense(g.Key, g.Sum(o => o.Amount)));

            return new AnalyticsResult<CategoryExpense>(grouped);
        }
    }
}