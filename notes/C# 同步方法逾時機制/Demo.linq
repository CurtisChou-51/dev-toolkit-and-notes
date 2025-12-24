<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async void Main()
{
    Console.WriteLine("Demo Start");
    await Demo();
    Console.WriteLine("Demo End");
}

async Task Demo()
{
    try
    {
        await Task.Run(DoSomething).WaitAsync(TimeSpan.FromSeconds(5));
    }
    catch (TimeoutException)
    {
        Console.WriteLine("Timeout!");
    }
}

void DoSomething()
{
    Console.WriteLine("DoSomething Start");
    Thread.Sleep(7000);
    Console.WriteLine("DoSomething End (雖然逾時了但其實還在執行)");
}
