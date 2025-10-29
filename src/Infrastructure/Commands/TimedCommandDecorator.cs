using System.Diagnostics;

namespace Infrastructure.Commands
{
    public class TimedCommandDecorator(ICommand inner, Action<TimeSpan> onMeasured) : ICommand
    {
        public async Task ExecuteAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            await inner.ExecuteAsync();
            sw.Stop();
            onMeasured(sw.Elapsed);
        }
    }
}