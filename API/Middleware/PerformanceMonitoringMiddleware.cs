using Application;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Wolverine;

namespace API.Middleware;

public class PerformanceMonitoringMiddleware
{
    
    public static Stopwatch Before(IMessageContext context)
    {
        var commandName = context.Envelope.MessageType.Split('.').LastOrDefault();

        var sw = Stopwatch.StartNew();
        Helpers.PrintTimestamp($"[PERF] >>> {commandName} started");
        return sw;
    }

    public static void Finally(Stopwatch sw, IMessageContext context)
    {
        var commandName = context.Envelope.MessageType.Split('.').LastOrDefault();

        sw.Stop();
        Helpers.PrintTimestamp($"[PERF] <<< {commandName} took {sw.ElapsedMilliseconds} ms");
    }
}

