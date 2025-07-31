using Venice.Application.Interfaces;
using Venice.Application.Services;
using Venice.Domain.Interfaces;
using Venice.Domain.Repositories;
using Venice.Domain.Settings;
using Venice.Infra.Cache;
using Venice.Infra.Messaging;
using Venice.Infra.Repositories;

namespace Venice.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositórios e serviços
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderService, OrderService>();

        // Cache
        services.AddScoped<ICacheRepository, RedisCacheRepository>();

        // Mensageria
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));

        return services;
    }
}
