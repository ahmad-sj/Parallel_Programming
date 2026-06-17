namespace Infrastructure.Middlewares;

using Application;
using Application.Interfaces;
using RedLockNet;
using Wolverine;

public static class DistributedLockMiddleware {
    public record RedisLockToken(IRedLock Lock);
    public static async Task<(HandlerContinuation, RedisLockToken?)> BeforeAsync(
        object message,
        IDistributedLockFactory lockFactory,
        CancellationToken cancellationToken)
    {
        // Only apply to lockable commands
        if (message is not ILockableCommand command)
            return (HandlerContinuation.Continue, null);

        var resourceKey = $"{command.LockKey}";
        var expiryTime = TimeSpan.FromSeconds(30);
        var waitTime = TimeSpan.FromSeconds(10);
        var retryTime = TimeSpan.FromSeconds(3);
        var redLock = await lockFactory.CreateLockAsync(
            resourceKey, expiryTime, waitTime, retryTime, cancellationToken);

        if (!redLock.IsAcquired) {
            Helpers.PrintTimestamp($"Distributed Lock busy for: '{resourceKey}'. Skipping.");
            return (HandlerContinuation.Stop, null);
        }
        Helpers.PrintTimestamp($"Distributed Lock ACQUIRED for: '{resourceKey}'");
        return (HandlerContinuation.Continue, new RedisLockToken(redLock));
    }

    public static async Task FinallyAsync(RedisLockToken? token) {
        if (token != null) {
            await token.Lock.DisposeAsync();
            Helpers.PrintTimestamp($"Distributed Lock RELEASED.");
        }
    }
}


public class ResourceLockedException : Exception {
    public ResourceLockedException(string resourceKey)
        : base($"Resource '{resourceKey}' is currently locked distributedly.") {}
}