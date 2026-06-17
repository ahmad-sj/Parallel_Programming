using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Services.DailySales;
public class DailySalesParallelProcesser {
    private readonly IRepository _repository;
    public DailySalesParallelProcesser(IRepository repository)
    {
        _repository = repository;
    }
    public async Task ProcessSalesAsync(CancellationToken stoppingToken) {
        Helpers.PrintTimestamp("Fetching orders in pages from database ...");
        int totalOrders = await _repository.GetCountAsync<Order>();
        Helpers.PrintTimestamp($"Total orders count: {totalOrders}");

        int pageSize = 20;
        Helpers.PrintTimestamp($"Page size: {pageSize}");

        // Getting orders in pages
        int page = 0;
        var pages = new List<List<Order>>();
        while (!stoppingToken.IsCancellationRequested) {
            var ordersPage = await _repository.GetQueryable<Order>()
                .Include(i => i.Items)
                .OrderBy(o => o.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(stoppingToken);

            if (!ordersPage.Any()) break;

            int currentPageNo = page + 1;
            pages.Add(ordersPage);
            Helpers.PrintTimestamp($"Page #{currentPageNo} fetched");
            page++;
        }

        // processing pages concurrently
        double totalSales = 0;
        object lockObj = new();
        var t0 = Stopwatch.GetTimestamp();
        await Parallel.ForEachAsync(pages.Select((value, index) => new { value, index }),
            async (item, ct) => {
                var page = item.value;
                int pageNo = item.index + 1;
                var pageTotal = ProcessPage(page, pageNo);
                lock (lockObj) {
                    totalSales += pageTotal;
                }
            });

        var t1 = Stopwatch.GetTimestamp();
        Helpers.PrintTimestamp($"Total sales amount: {Math.Round(totalSales, 2)}");
        Helpers.PrintTimestamp($"Processing time: {Stopwatch.GetElapsedTime(t0, t1)
            .TotalMilliseconds} ms");
    }
    private double ProcessPage(List<Order> ordersPage, int PageNo) {
        Helpers.PrintTimestamp($"Processing page #{PageNo} on thread " +
            $"{Thread.CurrentThread.ManagedThreadId}");
        Task.Delay(3000).Wait();
        return ordersPage.Sum(order => order.GetTotalPrice());
    }
}

