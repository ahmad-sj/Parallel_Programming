using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Services;

public class OrderSalesProcesser
{
    private readonly IRepository _repository;

    public OrderSalesProcesser(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ProcessSalesAsync(CancellationToken stoppingToken)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");

        Console.WriteLine("\n" + timeWithMS + "\tBatches Creating Started");

        const int batchSize = 5;

        int page = 0;
        double total = 0;

        var batches = new List<List<Order>>();

        while (true)
        {
            var ordersBatch = await _repository.GetQueryable<Order>()
                .Include(i => i.Items)
                .OrderBy(o => o.Id)
                .Skip(page * batchSize)
                .Take(batchSize)
                .ToListAsync();

            if (!ordersBatch.Any())
                break;


            batches.Add(ordersBatch);

            timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine($"\n{timeWithMS}\tBatch #{(page + 1)} created");

            await Task.Delay(1000, stoppingToken);

            page++;
        }

        Console.WriteLine("\n============================================");

        timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine($"\n{timeWithMS}\tBatches Processing started");

        object lockObj = new();
        await Parallel.ForEachAsync(batches.Select((value, index) => new { value, index }), async (item, ct) =>
        {
            var batch = item.value;
            int batchNo = item.index + 1; // Adding 1 so it's 1-based for the console

            string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine($"\n{timeWithMS}\tProcessing batch #{batchNo}");

            var batchTotal = await ProcessBatchAsync(batch);

            Console.WriteLine($"\nBatch #{batchNo} total is: {batchTotal}");

            lock (lockObj)
            {
                total += batchTotal;
            }
        });

        Console.WriteLine("\n============================================");

        timeWithMS = DateTime.Now.ToString("HH:mm:ss.fff");
        Console.WriteLine($"\n{timeWithMS}\tTotal sales amount: {total}");
    }

    public async Task<double> ProcessBatchAsync(List<Order> ordersBatch)
    {
        double total = 0;
        object lockObj = new();

        await Parallel.ForEachAsync(
            ordersBatch,
            async (order, token) =>
            {
                lock (lockObj)
                {
                    total += order.GetTotalPrice();
                }
            });
        return total;
    }
}
