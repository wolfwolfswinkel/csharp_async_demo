using System;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_async_demo
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("Main() enter");

                var runCts = new CancellationTokenSource();

                Console.CancelKeyPress += (sender, args) => {
                    runCts.Cancel();
                    args.Cancel = true; // Prevent event propagation; we should get normal program exit
                };


                var periodicJob = new PeriodicJob(10);

                Console.WriteLine("Starting periodicJob");

                var periodicJobTask = periodicJob.Run(runCts.Token);

                Console.WriteLine("Waiting for Ctrl-C and all tasks to finish");
                await periodicJobTask;

                Console.WriteLine("Main() end");
                return 0;
            }
            catch ( Exception e )
            {
                Console.WriteLine($"Main caught {e}");
                return 1;
            }
        }
    }
}
