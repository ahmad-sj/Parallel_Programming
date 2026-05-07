using Microsoft.Extensions.Logging;

namespace Application.Notifications;

public class SendOrderNotificationHandler
{
    private readonly ILogger<SendOrderNotificationHandler> _logger;

    public SendOrderNotificationHandler(ILogger<SendOrderNotificationHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SendOrderNotificationCommand command)
    {
        _logger.LogInformation("⚠️ Background task STARTED at: {Time}", DateTime.Now.ToString("HH:mm:ss"));
        _logger.LogInformation("⏳ Will complete after 10 seconds...");

        await Task.Delay(100);  // 10 ثواني

        _logger.LogInformation("✅ Background task COMPLETED at: {Time}", DateTime.Now.ToString("HH:mm:ss"));
        _logger.LogInformation("📧 Notification Sent To User: {UserId}", command.UserId);
        _logger.LogInformation("📦 Product: {ProductName}", command.ProductName);
        _logger.LogInformation("🔢 Quantity: {Qty}", command.Qty);
    }
}