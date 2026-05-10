using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================
// الملف: Application/Services/OrderNotificationQueue.cs
// ============================================================
// نقلنا الـ Queue من API.Services إلى Application
// لأن Application لا يعرف API (dependency تسير من API → Application)
// ============================================================

using System.Threading.Channels;
using Application.Notifications;

namespace Application.Services;   // ← namespace جديد

public class OrderNotificationQueue
{
    private readonly Channel<SendOrderNotificationCommand> _channel =
        Channel.CreateBounded<SendOrderNotificationCommand>(
            new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

    public async ValueTask EnqueueAsync(SendOrderNotificationCommand command,
                                         CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(command, ct);
    }

    public IAsyncEnumerable<SendOrderNotificationCommand> ReadAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}
