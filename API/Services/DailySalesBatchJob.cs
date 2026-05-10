// ============================================================
// المتطلب 4: معالجة البيانات الضخمة على دفعات (Batch Processing)
// الملف: API/Services/DailySalesBatchJob.cs
// ============================================================
// BackgroundService يعمل كل يوم في وقت محدد
// يقرأ بيانات الطلبات على دُفعات (chunks) بدل تحميلها كلها دفعة واحدة
// هذا يحمي الـ RAM ويسرّع المعالجة
// ============================================================

using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Services;

public class DailySalesBatchJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailySalesBatchJob> _logger;

    // ✅ حجم كل Chunk → عدّله حسب حجم قاعدة بياناتك وموارد السيرفر
    private const int ChunkSize = 50;

    public DailySalesBatchJob(
        IServiceScopeFactory scopeFactory,
        ILogger<DailySalesBatchJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📊 DailySalesBatchJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // ── احسب الوقت حتى الـ 2:00 صباحاً القادمة ──────────────
            var now = DateTime.Now;
            var nextRun = now.Date.AddDays(now.Hour >= 2 ? 1 : 0).AddHours(2);
            var delay = nextRun - now;

            _logger.LogInformation(
                "⏰ Next batch run at {NextRun} (in {Minutes} min)",
                nextRun, (int)delay.TotalMinutes);

            //// ── انتظر حتى موعد التشغيل ────────────────────────────────
            //await Task.Delay(delay, stoppingToken);

            //// ── نفّذ المعالجة ──────────────────────────────────────────
            //await ProcessDailySalesAsync(stoppingToken);
            // في ExecuteAsync بدل حساب وقت الـ 2AM، شغّله بعد 5 ثواني من البداية:
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            await ProcessDailySalesAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken); // لا تكرر
        }
    }

    private async Task ProcessDailySalesAsync(CancellationToken ct)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation(
            "🔄 [BATCH] Daily sales processing STARTED at {Time}", startTime);

       // var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        int totalProcessed = 0;
        int chunkIndex = 0;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EcomDbContext>();

        // ── حساب إجمالي السجلات أولاً ────────────────────────────────
        //var totalOrders = await db.Orders
        //    .Where(o => o.Date.Date == yesterday)
        //    .CountAsync(ct);
        var totalOrders = await db.Orders.CountAsync(ct);

        _logger.LogInformation(
            "📋 [BATCH] Total orders to process: {Total}", totalOrders);

        if (totalOrders == 0)
        {
            // _logger.LogInformation("ℹ️ [BATCH] No orders for yesterday. Skipping.");
            _logger.LogInformation("ℹ️ [BATCH] No orders found in DB. Add orders first!");
            return;
        }

        // ── المعالجة على دفعات (Chunks) ──────────────────────────────
        // Skip/Take بدل تحميل كل البيانات مرة واحدة
        while (totalProcessed < totalOrders && !ct.IsCancellationRequested)
        {
            //var chunk = await db.Orders
            //    .Where(o => o.Date.Date == yesterday)
            //    .Include(o => o.Items)
            var chunk = await db.Orders
    
                .OrderBy(o => o.Date)
                .Skip(chunkIndex * ChunkSize)
                .Take(ChunkSize)
                .ToListAsync(ct);

            if (!chunk.Any()) break;

            chunkIndex++;
            _logger.LogInformation(
                "  ⚙️  [BATCH] Processing chunk #{Chunk} | {Count} orders...",
                chunkIndex, chunk.Count);

            // ── معالجة كل Chunk ───────────────────────────────────────
            var chunkSummary = await ProcessChunkAsync(chunk, db, ct);

            totalProcessed += chunk.Count;

            _logger.LogInformation(
                "  ✅ [BATCH] Chunk #{Chunk} done | Revenue={Revenue:C2} | Total so far: {Total}/{Grand}",
                chunkIndex, chunkSummary.TotalRevenue, totalProcessed, totalOrders);

            // تأخير صغير بين الـ chunks لتخفيف الضغط على DB
            await Task.Delay(200, ct);
        }

        var elapsed = DateTime.UtcNow - startTime;
        _logger.LogInformation(
            "🏁 [BATCH] COMPLETED | Processed={Total} orders in {Elapsed:hh\\:mm\\:ss}",
            totalProcessed, elapsed);
    }

    private async Task<ChunkSummary> ProcessChunkAsync(
        List<Order> orders,
        EcomDbContext db,
        CancellationToken ct)
    {
        double totalRevenue = 0;

        foreach (var order in orders)
        {
            // ── هنا تضع المنطق الفعلي لكل طلب ────────────────────────
            // مثلاً: حساب الإيرادات، تحديث تقارير المخزون، إرسال تقارير...

            //  var orderRevenue = order.Items?.Sum(i => i.Qty * i.UnitPrice) ?? 0;
            var orderRevenue = 0.0; // سنحسبها لاحقاً بعد إضافة Migration

            totalRevenue += orderRevenue;

            // مثال: تحديث حقل "Processed" في Order
            // order.IsProcessed = true;
            // db.Orders.Update(order);
        }

        // await db.SaveChangesAsync(ct); // فعّلها لو تحدّث البيانات

        return new ChunkSummary { TotalRevenue = totalRevenue };
    }

    // DTO بسيط لتجميع نتيجة كل Chunk
    private record ChunkSummary
    {
        public double TotalRevenue { get; init; }
    }
}