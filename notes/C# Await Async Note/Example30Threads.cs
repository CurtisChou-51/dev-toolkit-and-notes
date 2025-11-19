using System.Diagnostics;

ThreadPool.SetMinThreads(30, 1);
ThreadPool.SetMaxThreads(30, 1);

Stopwatch sw = Stopwatch.StartNew();
var tasks = Enumerable.Range(1, 100).Select(_ => Task.Run(DoWork)).ToArray();
Task.WaitAll(tasks);
sw.Stop();
Console.WriteLine($"All tasks completed. Elapsed = {sw.Elapsed}");

sw.Restart();
var asyncTasks = Enumerable.Range(1, 100).Select(_ => Task.Run(DoWorkAsync)).ToArray();
Task.WaitAll(asyncTasks);
sw.Stop();
Console.WriteLine($"All asyncTasks completed. Elapsed = {sw.Elapsed}");

void DoWork()
{
    Thread.Sleep(3000);
}

async Task DoWorkAsync()
{
    await Task.Delay(3000);
}