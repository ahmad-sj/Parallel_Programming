// ============================================================
// Program.cs - الأسطر المضافة فقط (✅)
// ============================================================

using API.Services;
using Application.Admin.Products.CreateProduct;
using Application.Services;   // ← أضف هذا
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Wolverine;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.UseWolverine(opt =>
        {
            opt.Discovery.IncludeAssembly(typeof(CreateProductHandler).Assembly);
        });

        // ✅ المتطلب 3
        builder.Services.AddSingleton<OrderNotificationQueue>();
        builder.Services.AddHostedService<OrderNotificationBackgroundService>();

        // ✅ المتطلب 4
        builder.Services.AddHostedService<DailySalesBatchJob>();

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromSeconds(10);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 50;
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests", token);
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRateLimiter();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers().RequireRateLimiting("fixed");

        app.Run();
    }
}