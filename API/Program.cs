using Application.Admin.Products.AddProduct;
using Application.Services;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;
using Wolverine;
using Wolverine.SqlServer;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<OrderSalesProcesser>();

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.UseWolverine(opt =>
        {
            opt.Discovery.IncludeAssembly(typeof(AddProductHandler).Assembly);

            //opt.PersistMessagesWithSqlServer();

            //opt.Policies.UseDurableLocalQueues();
        });

        //builder.Host.UseResourceSetupOnStartup();

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.PermitLimit = 1; // max requests
                opt.Window = TimeSpan.FromSeconds(10); // per time window
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0; // 0 = drop extra requests immediately
            });

            // Optional: custom response when rejected
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests", token);
            };
        });

        var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "EcomCache_"; // Prefix for keys stored in Redis
        });

        var app = builder.Build();

        app.MigrateAsync().Wait();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseRateLimiter();

        bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        if (!isDocker)
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthorization();

        //app.MapControllers().RequireRateLimiting("fixed");
        app.MapControllers();

        app.Run();
    }
}
