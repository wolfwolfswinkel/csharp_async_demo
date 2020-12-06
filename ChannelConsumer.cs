using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace csharp_async_demo
{
    public class ChannelConsumer
    {
        private readonly ChannelReader<string> _reader;

        public ChannelConsumer(ChannelReader<string> reader)
        {
            _reader = reader;
        }

        public async Task RunAsync(CancellationToken ct)
        {
            try
            {
               while ( ! ct.IsCancellationRequested )
               {
                    string message = await _reader.ReadAsync(ct);

                    Console.WriteLine($"Consumed {message}");

                    // artificial slow consumer
                    await Task.Delay(TimeSpan.FromSeconds(2), ct);
               }
            }
            catch (ChannelClosedException)
            {
                // When end of channel is reached
            }
            catch (OperationCanceledException)
            {
                // Consider cancellation as normal exit
            }


            // Similar using ReadAllAsync()
            /*
            try
            {
                // Note: ReadAllAsync() will read to the end of the channel
                // even if cancelled, so explicitly check token in the loop.
               await foreach ( string message in _reader.ReadAllAsync(ct) )
               {
                    ct.ThrowIfCancellationRequested();

                    Console.WriteLine($"Consumed {message}");

                    // artificial slow consumer
                    await Task.Delay(TimeSpan.FromSeconds(2), ct);
               }
            }
            catch (OperationCanceledException)
            {
                // Consider cancellation as normal exit
            }
            */
        }
    }
}