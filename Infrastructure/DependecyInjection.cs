using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using TanvirArjel.EFCore.GenericRepository;

namespace Infrastructure;

public static class DependecyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddAppRepository();
        services.AddRedis(configuration);
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EcomDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")
            ));

        //.AddIdentity<User, IdentityRole>();
        return services;
    }

    public static async Task MigrateAsync(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EcomDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static IServiceCollection AddAppRepository(this IServiceCollection services)
    {
        services.AddGenericRepository<EcomDbContext>();
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnection");

        // Parse the string and disable immediate crashes on startup
        var configOptions = ConfigurationOptions.Parse(redisConnectionString!);
        configOptions.AbortOnConnectFail = false;

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = configOptions;
            options.InstanceName = "EcomCache_"; // Prefix for keys stored in Redis
        });

        // Setup StackExchange.Redis ConnectionMultiplexer
        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);

        // Create the RedLock endpoints list
        var endPoints = new List<RedLockMultiplexer> { multiplexer };

        // Register as Singleton so Wolverine can fetch it during execution
        services.AddSingleton<IDistributedLockFactory>(sp =>
            RedLockFactory.Create(endPoints)); 
        
        return services;
    }
}
