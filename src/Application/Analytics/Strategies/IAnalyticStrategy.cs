namespace Application.Analytics.Strategies
{
    public interface IAnalyticsStrategy
    {
        string Name { get; }
        Task<IAnalyticsResult> AnalyzeAsync();
    }

}