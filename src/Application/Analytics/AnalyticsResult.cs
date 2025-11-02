namespace Application.Analytics
{
    public class AnalyticsResult<T>(IEnumerable<T> data) : IAnalyticsResult
    {
        public IReadOnlyCollection<T> Data { get; } = data.ToList();

        public void Accept(IAnalyticsResultVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}