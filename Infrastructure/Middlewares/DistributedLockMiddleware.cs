namespace Infrastructure.Middlewares;

using Application.Interfaces;
using RedLockNet;
using Wolverine;

public static class DistributedLockMiddleware
{
    public record RedisLockToken(IRedLock Lock);

    public static async Task<(HandlerContinuation, RedisLockToken?)> BeforeAsync(
        object message,
        IDistributedLockFactory lockFactory,
        CancellationToken cancellationToken)
    {
        // Only apply to lockable commands
        if (message is not ILockableCommand command)
        {
            return (HandlerContinuation.Continue, null);
        }

        var resourceKey = $"{command.LockKey}";

        var expiryTime = TimeSpan.FromSeconds(30);
        var waitTime = TimeSpan.Zero;
        var retryTime = TimeSpan.FromMilliseconds(10);

        var redLock = await lockFactory.CreateLockAsync(
            resourceKey,
            expiryTime,
            waitTime,
            retryTime,
            cancellationToken);

        if (!redLock.IsAcquired)
        {
            Console.WriteLine($"[{Environment.MachineName}] Lock busy for: '{resourceKey}'. Skipping.");
            return (HandlerContinuation.Stop, null);
            //throw new ResourceLockedException(resourceKey);
        }

        Console.WriteLine($"[{Environment.MachineName}] Lock ACQUIRED for: '{resourceKey}'");
        return (HandlerContinuation.Continue, new RedisLockToken(redLock));
    }

    public static async Task FinallyAsync(RedisLockToken? token)
    {
        if (token != null)
        {
            await token.Lock.DisposeAsync();
            Console.WriteLine($"[{Environment.MachineName}] Lock RELEASED.");
        }
    }
}


public class ResourceLockedException : Exception
{
    public ResourceLockedException(string resourceKey)
        : base($"Resource '{resourceKey}' is currently locked.")
    {
    }
}