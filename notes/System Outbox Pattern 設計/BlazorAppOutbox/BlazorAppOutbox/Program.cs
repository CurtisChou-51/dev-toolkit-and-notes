using BlazorAppOutbox.Components;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppOutbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // 註冊 In-Memory DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("AppDb"));

            builder.Services.AddScoped<OrderProcessService>();
            builder.Services.AddHostedService<OrderOutboxBackgroundService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
