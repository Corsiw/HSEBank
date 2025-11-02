using Domain.Interfaces;

namespace Domain.Analytics
{
    public interface IAnalyticsResult
    {
        void Accept(IAnalyticsResultVisitor visitor);
    }
}