// ============================================================
// الملف: API/Services/OrderNotificationBackgroundService.cs (مُصلَح)
// ============================================================

using Application.Notifications;
using Application.Services;   // ← من Application مش API.Services
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Services;

public class OrderNotificationBackgroundService : BackgroundService
{
    private readonly OrderNotificationQueue _queue;
    private readonly ILogger<OrderNotificationBackgroundService> _logger;

    public OrderNotificationBackgroundService(
        OrderNotificationQueue queue,
        ILogger<OrderNotificationBackgroundService> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 NotificationBackgroundService started...");

        await foreach (var command in _queue.ReadAllAsync(stoppingToken))
        {
            _ = Task.Run(() => ProcessAsync(command, stoppingToken), stoppingToken);
        }
    }

    private async Task ProcessAsync(SendOrderNotificationCommand command, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation(
                "📩 [BACKGROUND] Processing notification | User={UserId} | Product={Product} | Time={Time}",
                command.UserId, command.ProductName, DateTime.Now.ToString("HH:mm:ss.fff"));

            await Task.Delay(10_000, ct); // محاكاة إرسال Email/PDF

            _logger.LogInformation(
                "✅ [BACKGROUND] Notification done | User={UserId} | Time={Time}",
                command.UserId, DateTime.Now.ToString("HH:mm:ss.fff"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Notification failed for User={UserId}", command.UserId);
        }
    }
}