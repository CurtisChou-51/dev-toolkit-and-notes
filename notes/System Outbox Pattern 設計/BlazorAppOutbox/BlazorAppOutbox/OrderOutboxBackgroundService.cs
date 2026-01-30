namespace BlazorAppOutbox
{
    public class OrderOutboxBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderOutboxBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<OrderProcessService>();
                    await processor.ProcessOutboxAsync();
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}   