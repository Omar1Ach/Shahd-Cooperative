using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ShahdCooperative.Infrastructure.Messaging;

public class MessagePublisher : IDisposable
{
    private readonly RabbitMQConnection _rabbitMQConnection;
    private readonly ILogger<MessagePublisher> _logger;
    private IChannel? _channel;
    private bool _disposed;

    public MessagePublisher(RabbitMQConnection rabbitMQConnection, ILogger<MessagePublisher> logger)
    {
        _rabbitMQConnection = rabbitMQConnection;
        _logger = logger;
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message)
    {
        try
        {
            var channel = await GetChannelAsync();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "Published message to exchange '{Exchange}' with routing key '{RoutingKey}'",
                exchange,
                routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error publishing message to exchange '{Exchange}' with routing key '{RoutingKey}'",
                exchange,
                routingKey);
            throw;
        }
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (_channel != null && _channel.IsOpen)
            return _channel;

        var connection = await _rabbitMQConnection.GetConnectionAsync();
        _channel = await connection.CreateChannelAsync();

        return _channel;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
            _disposed = true;
        }
    }
}
