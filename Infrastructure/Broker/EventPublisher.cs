using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Infrastructure.Broker
{
    /// <summary>
    /// RabbitMQ implementation of the event publisher.
    /// This follows a singleton pattern for the connection to avoid performance issues.
    /// </summary>
    public class EventPublisher : IEventPublisher, IDisposable
    {
        private readonly ILogger<EventPublisher> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                await EnsureConnectionAsync();

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                await _channel!.QueueDeclareAsync(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null,
                                     cancellationToken: cancellationToken);

                await _channel.BasicPublishAsync(exchange: string.Empty,
                                    routingKey: queueName,
                                    body: body,
                                    cancellationToken: cancellationToken);

                _logger.LogInformation("[RabbitMQ] Message published to queue: {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error publishing message to {QueueName}", queueName);
                throw;
            }
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                var factory = new ConnectionFactory { HostName = "localhost" }; // Configurable via appsettings
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
