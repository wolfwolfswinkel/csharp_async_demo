using System;
using System.Threading;
using System.Threading.Tasks;

public class PeriodicJob
{
    private readonly int _intervalSeconds;
    private int _runCount;

    public PeriodicJob(int intervalSeconds)
    {
        _intervalSeconds = intervalSeconds;
    }
    public async Task Run(CancellationToken ct)
    {
        try
        {
            while ( !ct.IsCancellationRequested )
            {
                ++_runCount;
                Console.WriteLine($"Periodic job run {_runCount}");
                await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), ct);
            }
        }
        catch (TaskCanceledException)
        {
            // Consider cancellation as normal exit
        }
    }
}