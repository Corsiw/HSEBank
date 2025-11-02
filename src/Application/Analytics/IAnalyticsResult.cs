namespace Application.Analytics
{
    public interface IAnalyticsResult
    {
        void Accept(IAnalyticsResultVisitor visitor);
    }
}