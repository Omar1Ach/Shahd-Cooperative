using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ShahdCooperative.Infrastructure.Messaging;

public abstract class MessageConsumer<T> : IDisposable
{
    private readonly RabbitMQConnection _rabbitMQConnection;
    protected readonly ILogger Logger;
    private IChannel? _channel;
    private bool _disposed;

    protected MessageConsumer(RabbitMQConnection rabbitMQConnection, ILogger logger)
    {
        _rabbitMQConnection = rabbitMQConnection;
        Logger = logger;
    }

    public async Task StartConsuming(string queueName)
    {
        try
        {
            var channel = await GetChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(messageJson);

                    if (message != null)
                    {
                        await ProcessMessageAsync(message);
                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        Logger.LogInformation(
                            "Successfully processed message from queue '{QueueName}'",
                            queueName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        ex,
                        "Error processing message from queue '{QueueName}'",
                        queueName);

                    await channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            Logger.LogInformation("Started consuming from queue '{QueueName}'", queueName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting consumer for queue '{QueueName}'", queueName);
            throw;
        }
    }

    protected abstract Task ProcessMessageAsync(T message);

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
