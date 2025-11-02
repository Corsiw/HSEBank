namespace Application.Analytics
{
    public interface IAnalyticsResultVisitor
    {
        void Visit<T>(AnalyticsResult<T> result);
    }
}