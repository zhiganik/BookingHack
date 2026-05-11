using BookingHack.Application.Repositories;
using BookingHack.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BookingHack.Infrastructure.Redis;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["Redis:Connection"]
            ?? throw new InvalidOperationException("Redis connection string is not configured.");

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(connectionString));

        services.AddScoped<IRefreshTokenRepository, RedisRefreshTokenRepository>();

        return services;
    }
}
