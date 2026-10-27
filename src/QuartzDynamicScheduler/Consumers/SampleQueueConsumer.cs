using Microsoft.Extensions.Options;
using QuartzDynamicScheduler.Configurations;
using QuartzDynamicScheduler.Domain.Models;
using QuartzDynamicScheduler.Infrastructure.Broker;

namespace QuartzDynamicScheduler.Consumers;

public sealed class SampleQueueConsumer : EventConsumerBase
{
    public SampleQueueConsumer(
        RabbitMqConnection connection,
        IOptions<MessageBrokerOptions> options,
        ILogger<SampleQueueConsumer> logger)
        : base(QueueEventType.SampleQueueEvent, connection, options, logger)
    {
    }

    protected override Task HandleAsync(MessageEventModel message, CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processed {MessageType} message {MessageId} created at {CreatedAt} (reprocess: {Reprocess}): {Payload}",
            message.MessageType,
            message.Id,
            message.CreatedAt,
            message.Reprocess,
            message.Message);

        return Task.CompletedTask;
    }
}
