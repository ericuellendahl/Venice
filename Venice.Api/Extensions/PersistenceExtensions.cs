using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using StackExchange.Redis;
using Venice.Domain.Settings;
using Venice.Infra.Data;

namespace Venice.Api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Redis
        var redisSettings = configuration.GetSection("Redis").Get<RedisSettings>();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = new ConfigurationOptions
            {
                EndPoints = { { redisSettings.Host, redisSettings.Port } },
                User = redisSettings.User,
                Password = redisSettings.Password,
                AbortOnConnectFail = false
            };

            return ConnectionMultiplexer.Connect(options);
        });

        // PostgreSQL
        var postgresConn = configuration.GetConnectionString("Postgres");
        services.AddDbContext<VeniceDbContext>(options =>
            options.UseNpgsql(postgresConn, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "public"))
                   .UseSnakeCaseNamingConvention());

        // MongoDB
        var mongoConn = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        services.AddSingleton(provider =>
            new MongoDbContext(mongoConn, "VeniceOrders"));

        return services;
    }
}
