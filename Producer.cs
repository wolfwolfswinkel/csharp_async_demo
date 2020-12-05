using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace csharp_async_demo
{
    public class Producer
    {
        private readonly ChannelWriter<string> _writer;
        private int _count;

        public Producer(ChannelWriter<string> writer)
        {
            _writer = writer;
        }

        public async Task Run(CancellationToken ct)
        {
            try
            {
                var rnd = new Random();

                while ( !ct.IsCancellationRequested )
                {
                    ++_count;
                    string message = $"Message{_count}";
                    Console.WriteLine($"Producing {message}");

                    await _writer.WriteAsync(message, ct);
                    Console.WriteLine($"Produced {message}");
                    
                    await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(500, 1000)), ct);
                }
            }
            catch (OperationCanceledException)
            {
                // Consider cancellation as normal exit
            }
            finally
            {
                Console.WriteLine("Producer marking writer complete");
                _writer.Complete();
            }
        }
    }
}