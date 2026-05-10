using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class SalesAggregationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SalesAggregationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            var nextRun = DateTime.Today.AddDays(1).AddHours(2);

            var delay = nextRun - now;

            Console.WriteLine("Next batch job scheduled after {Delay}",
                delay);

            await Task.Delay(delay, stoppingToken);

            var service = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<OrderSalesProcesser>();
            
            await service.ProcessSalesAsync(stoppingToken);
        }
    }
}
