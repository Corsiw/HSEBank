using Domain.Analytics;

namespace Domain.Interfaces
{
    public interface IAnalyticsResultVisitor
    {
        void Visit<T>(AnalyticsResult<T> result);
    }
}