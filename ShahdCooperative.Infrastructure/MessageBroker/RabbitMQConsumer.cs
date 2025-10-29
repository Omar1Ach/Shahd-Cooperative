using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Handlers;
using System.Text;
using System.Text.Json;

namespace ShahdCooperative.Infrastructure.MessageBroker;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQConsumer(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQConsumer> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await InitializeRabbitMQAsync(stoppingToken);

            _logger.LogInformation("RabbitMQ Consumer started and listening for events");

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ Consumer is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in RabbitMQ Consumer");
            throw;
        }
    }

    private async Task InitializeRabbitMQAsync(CancellationToken stoppingToken)
    {
        var host = _configuration["RabbitMQ:Host"] ?? "localhost";
        var port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672");
        var username = _configuration["RabbitMQ:Username"] ?? "guest";
        var password = _configuration["RabbitMQ:Password"] ?? "guest";
        var virtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/";
        var exchange = _configuration["RabbitMQ:Exchange"] ?? "shahdcooperative.events";

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            VirtualHost = virtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Declare exchange
        await _channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Declare queue for this service
        var queueName = "shahdcooperative.main.queue";
        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind queue to exchange with routing keys
        await _channel.QueueBindAsync(queueName, exchange, "user.registered");
        await _channel.QueueBindAsync(queueName, exchange, "user.logged-in");
        await _channel.QueueBindAsync(queueName, exchange, "user.logged-out");

        _logger.LogInformation(
            "Connected to RabbitMQ at {Host}:{Port}, listening to queue {Queue}",
            host,
            port,
            queueName);

        // Set up consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation(
                    "Received message with routing key {RoutingKey}",
                    routingKey);

                await ProcessMessageAsync(routingKey, message);

                // Acknowledge the message
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");

                // Reject and requeue the message
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task ProcessMessageAsync(string routingKey, string message)
    {
        using var scope = _serviceProvider.CreateScope();

        try
        {
            switch (routingKey)
            {
                case "user.registered":
                    var userRegisteredEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(message);
                    if (userRegisteredEvent != null)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<UserRegisteredEvent>>();
                        await handler.HandleAsync(userRegisteredEvent);
                    }
                    break;

                case "user.logged-in":
                    var userLoggedInEvent = JsonSerializer.Deserialize<UserLoggedInEvent>(message);
                    if (userLoggedInEvent != null)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<UserLoggedInEvent>>();
                        await handler.HandleAsync(userLoggedInEvent);
                    }
                    break;

                case "user.logged-out":
                    var userLoggedOutEvent = JsonSerializer.Deserialize<UserLoggedOutEvent>(message);
                    if (userLoggedOutEvent != null)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<UserLoggedOutEvent>>();
                        await handler.HandleAsync(userLoggedOutEvent);
                    }
                    break;

                default:
                    _logger.LogWarning("Unknown routing key: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message with routing key {RoutingKey}", routingKey);
            throw;
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
