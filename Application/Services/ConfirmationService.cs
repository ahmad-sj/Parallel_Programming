using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public static class ConfirmationService
{
    public static async Task SendEmail(int userId, int orderId)
    {
        var t0 = Stopwatch.GetTimestamp();
        Helpers.PrintTimestamp("Sending Confirmation...");

        await Task.Delay(4000);

        var t1 = Stopwatch.GetTimestamp();
        Helpers.PrintTimestamp($"Order '{orderId}' Confirmation Sent to User '{userId}' Successfully in {Stopwatch.GetElapsedTime(t0, t1).TotalMilliseconds} ms");
    }
}
