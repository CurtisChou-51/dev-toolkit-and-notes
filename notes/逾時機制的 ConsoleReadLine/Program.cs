using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("請輸入id，5秒內未輸入則進入自動執行模式");
            string? id = await ReadLineAsync(5000);
            if (string.IsNullOrEmpty(id))
                Console.WriteLine("自動執行模式開始");
            else
                Console.WriteLine($"手動執行 id = {id}");
            Console.ReadLine();
        }

        static Task<string?> ReadLineAsync(int timeOutMillisecs)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeOutMillisecs));
            return Task.Run(async () =>
            {
                try
                {
                    while (!Console.KeyAvailable)
                    {
                        if (cts.IsCancellationRequested)
                            return null;
                        Thread.Sleep(100);
                    }
                    return await Console.In.ReadLineAsync(cts.Token);
                }
                finally
                {
                    cts.Dispose();
                }
            });
        }

        static string? ReadLineAutoResetEvent(int timeOutMillisecs = Timeout.Infinite)
        {
            AutoResetEvent gotInput = new AutoResetEvent(false);
            string? input = string.Empty;
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeOutMillisecs));

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        input = await Console.In.ReadLineAsync(cts.Token);
                        gotInput.Set();
                    }
                }
                finally
                {
                    cts.Dispose();
                }
            }, cts.Token);

            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            return null;
        }
    }
}
