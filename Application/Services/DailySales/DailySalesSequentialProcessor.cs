using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Services.DailySales;
public class DailySalesSequentialProcessor {
    private readonly IRepository _repository;
    public DailySalesSequentialProcessor(IRepository repository)
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

        // processing pages sequentially
        double totalSales = 0;
        var t0 = Stopwatch.GetTimestamp();
        foreach (var tempPage in pages) {
            int pageNo = pages.IndexOf(tempPage) + 1;
            totalSales += ProcessPage(tempPage, pageNo);
        }

        var t1 = Stopwatch.GetTimestamp();
        Helpers.PrintTimestamp($"Total sales: {Math.Round(totalSales, 2)}");
        Helpers.PrintTimestamp($"Processing time: {Stopwatch.GetElapsedTime(t0, t1)
            .TotalMilliseconds} ms");
    }
    private double ProcessPage(List<Order> ordersPage, int pageNo) {
        Helpers.PrintTimestamp($"Processing page #{pageNo} on thread " +
            $"{Thread.CurrentThread.ManagedThreadId}");
        Task.Delay(3000).Wait();
        return ordersPage.Sum(order => order.GetTotalPrice());
    }
}


