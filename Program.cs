using System;
using System.Threading;
using System.Threading.Channels;
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


                var channel = Channel.CreateBounded<string>(
                    new BoundedChannelOptions(5) {
                            SingleReader = true,
                            SingleWriter = true,
                            FullMode= BoundedChannelFullMode.Wait
                    } );
                
                var producer = new Producer(channel.Writer);
                var consumer = new Consumer(channel.Reader);

                var producerTask = producer.Run(runCts.Token);
                
                // Attaching consumer to cancellation token terminates
                // it immediately, regardless of messages left in the channel
                // var consumerTask = consumer.Run(runCts.Token);
                
                // Not attaching to cancellation token makes it terminate
                // when producer completes channel
                var consumerTask = consumer.Run(default);


                Console.WriteLine("Waiting for Ctrl-C and all tasks to finish");
                await Task.WhenAll(periodicJobTask, producerTask, consumerTask);

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
