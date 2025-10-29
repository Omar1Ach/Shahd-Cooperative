using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ShahdCooperative.Domain.Interfaces;

namespace ShahdCooperative.Infrastructure.MessageBroker;

public class RabbitMQPublisher : IEventPublisher, IDisposable
{
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly string _exchange;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _channelLock = new(1, 1);

    public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
    {
        _logger = logger;
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672");
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";
        var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
        _exchange = configuration["RabbitMQ:Exchange"] ?? "shahdcooperative.events";

        InitializeConnectionAsync(host, port, username, password, virtualHost).GetAwaiter().GetResult();
    }

    private async Task InitializeConnectionAsync(string host, int port, string username, string password, string virtualHost)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password,
                VirtualHost = virtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(_exchange, ExchangeType.Topic, durable: true, autoDelete: false);

            _logger.LogInformation("RabbitMQ Publisher connected to {Host}:{Port}, Exchange: {Exchange}", host, port, _exchange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ Publisher");
            throw;
        }
    }

    public async Task PublishAsync<T>(string routingKey, T eventData, CancellationToken cancellationToken = default) where T : class
    {
        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel is not initialized");
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        }

        await _channelLock.WaitAsync(cancellationToken);
        try
        {
            var json = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel.BasicPublishAsync(
                exchange: _exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Published event to RabbitMQ: {RoutingKey}, Event: {EventType}", routingKey, typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event to RabbitMQ: {RoutingKey}", routingKey);
            throw;
        }
        finally
        {
            _channelLock.Release();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _channelLock.Dispose();
    }
}
