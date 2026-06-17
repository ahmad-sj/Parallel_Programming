using API.Middleware;
using Application.Interfaces;
using Application.Services.DailySales;
using Infrastructure;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using System.Reflection;
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

        // ============================================================
        // Daily sales processors registration

        builder.Services.AddScoped<DailySalesParallelProcesser>();
        builder.Services.AddScoped<DailySalesSequentialProcessor>();

        // ============================================================
        // Dependency injection for Infrastructure services

        builder.Services.AddInfrastructure(builder.Configuration);

        // ============================================================
        // Wolverine configuration

        builder.UseWolverine(opts =>
        {
            opts.PersistMessagesWithSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!, "wolverine");

            opts.Policies.UseDurableLocalQueues();

            // To include handlers from the Application assembly
            opts.Discovery.IncludeAssembly(Assembly.Load("Application"));

            // Apply the PerformanceMonitoringMiddleware to all handlers
            opts.Policies.AddMiddleware(typeof(PerformanceMonitoringMiddleware));

            // Apply the DistributedLockMiddleware to any handler
            // where the command implements ILockableCommand interface 
            opts.Policies
            .ForMessagesOfType<ILockableCommand>()
            .AddMiddleware(typeof(DistributedLockMiddleware));
        });

        // ============================================================
        // Rate Limiting configuration

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


        // ============================================================

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.MigrateAsync().Wait();

        // Global exception handling middleware
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception =
                    context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (exception is ResourceLockedException)
                {
                    context.Response.StatusCode = StatusCodes.Status423Locked;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exception.Message
                    });

                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unexpected error"
                });
            });
        });

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
