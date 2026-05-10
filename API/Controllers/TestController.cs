// ============================================================
// اختبار المتطلبين 3 و 4 - Controller مؤقت للاختبار
// الملف: API/Controllers/TestController.cs
// ============================================================
// هذا Controller فقط للاختبار، احذفه قبل التسليم أو احتفظ به للعرض التجريبي

using API.Services;
using Application.Notifications;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly OrderNotificationQueue _queue;
    private readonly DailySalesBatchJob _batchJob;
    private readonly ILogger<TestController> _logger;

    public TestController(
        OrderNotificationQueue queue,
        ILogger<TestController> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    // ────────────────────────────────────────────────────────────
    // اختبار المتطلب 3: إرسال عدة إشعارات وقياس سرعة الاستجابة
    // ────────────────────────────────────────────────────────────
    [HttpPost("test-async-queue")]
    public async Task<IActionResult> TestAsyncQueue([FromQuery] int count = 5)
    {
        var startTime = DateTime.Now;

        // أضف عدة مهام للـ Queue
        for (int i = 1; i <= count; i++)
        {
            await _queue.EnqueueAsync(new SendOrderNotificationCommand
            {
                UserId = Guid.NewGuid(),
                ProductName = $"Test Product #{i}",
                Qty = i * 2
            });
        }

        var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

        // ✅ النقطة الجوهرية: يجب أن يكون elapsed < 50ms
        // بينما المعالجة الفعلية (3000ms لكل مهمة) تحدث في الخلفية
        return Ok(new
        {
            Message = $"✅ {count} notifications enqueued successfully",
            ResponseTimeMs = elapsed,
            Note = "الإشعارات تُعالَج في الخلفية - المستخدم لم ينتظر",
            BackgroundWorking = "شاهد Console/Logs لرؤية المعالجة الخلفية"
        });
    }

    // ────────────────────────────────────────────────────────────
    // اختبار المتطلب 4: تشغيل الـ Batch Job يدوياً (للعرض فقط)
    // ────────────────────────────────────────────────────────────
    [HttpPost("trigger-batch-job")]
    public IActionResult TriggerBatchJob()
    {
        // في الإنتاج لا تفعل هذا - هنا للعرض التجريبي فقط
        _logger.LogInformation("🔧 [TEST] Batch job manually triggered via API");

        // نُطلق الـ job على Task مستقل
        _ = Task.Run(async () =>
        {
            _logger.LogInformation("🔄 [TEST-BATCH] Starting simulated batch processing...");

            // محاكاة معالجة 200 سجل على دفعات بحجم 50
            int total = 200, chunkSize = 50, processed = 0, chunk = 0;

            while (processed < total)
            {
                chunk++;
                int currentChunk = Math.Min(chunkSize, total - processed);
                await Task.Delay(500); // محاكاة عمل DB
                processed += currentChunk;

                _logger.LogInformation(
                    "  ⚙️  [TEST-BATCH] Chunk #{C} | {P}/{T} records processed",
                    chunk, processed, total);
            }

            _logger.LogInformation(
                "🏁 [TEST-BATCH] Completed: {Total} records in {Chunks} chunks",
                total, chunk);
        });

        return Accepted(new
        {
            Message = "✅ Batch job started in background",
            TotalRecords = 200,
            ChunkSize = 50,
            ExpectedChunks = 4,
            Note = "شاهد Console/Logs لمتابعة التقدم chunk by chunk"
        });
    }
}