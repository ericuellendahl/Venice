using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Venice.Application.Interfaces;
using Venice.Domain.Settings;

namespace Venice.Infra.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "venice-orders";

    public RabbitMqEventPublisher(IOptions<RabbitMQSettings> options)
    {
        var _settings = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost,
            Port = _settings.Port,
            Ssl = new SslOption
            {
                Enabled = _settings.SslEnabled,
                ServerName = _settings.SslServerName
            }
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);
    }

    public async Task PublishAsync<T>(T eventData) where T : class
    {
        var routingKey = typeof(T).Name;
        var message = JsonSerializer.Serialize(eventData);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
