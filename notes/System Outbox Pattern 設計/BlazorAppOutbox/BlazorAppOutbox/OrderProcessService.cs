using Microsoft.EntityFrameworkCore;

namespace BlazorAppOutbox
{
    public class OrderProcessService
    {
        private readonly AppDbContext _dbContext;
        private readonly Random random = new Random();

        public OrderProcessService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ProcessOutboxAsync()
        {
            var outboxes = await _dbContext.Outboxes
                .Where(x => (x.Status == 0 || x.Status == 2) && x.EventType == "OrderCreated")
                .OrderBy(x => x.Id)
                .Take(10)
                .ToListAsync();

            foreach (var outbox in outboxes)
                await SimulateProcessingAsync(outbox);
        }

        private async Task SimulateProcessingAsync(Outbox outbox)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == outbox.ReferenceId);
            if (order is null)
            {
                outbox.Status = 3;
                outbox.ProcessedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return;
            }

            // 模擬處理資料
            await Task.Delay(2000);
            int chance = random.Next(100);
            if (chance < 60)
            {
                // 60% 成功
                order.Status = outbox.Status = 1;
                order.ProcessedAt = outbox.ProcessedAt = DateTime.Now;
            }
            else if (chance < 90)
            {
                // 30% 重試
                order.Status = outbox.Status = 2;
                order.ProcessedAt = outbox.ProcessedAt = DateTime.Now;
            }
            else
            {
                // 10% 失敗不重試
                order.Status = outbox.Status = 3;
                order.ProcessedAt = outbox.ProcessedAt = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}