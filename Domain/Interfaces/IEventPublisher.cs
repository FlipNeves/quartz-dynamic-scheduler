namespace WorkerQuartzInteraction.Domain.Interfaces
{
    /// <summary>
    /// Interface for publishing events to a message broker.
    /// Supports decoupling business logic from RabbitMQ/ServiceBus implementation.
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class;
    }
}
