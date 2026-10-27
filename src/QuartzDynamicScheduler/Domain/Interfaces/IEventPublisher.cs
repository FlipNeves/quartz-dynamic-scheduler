using QuartzDynamicScheduler.Domain.Models;

namespace QuartzDynamicScheduler.Domain.Interfaces;

public interface IEventPublisher
{
    Task SendAsync(MessageRequestModel request, CancellationToken cancellationToken);
    Task SendAllAsync(IEnumerable<MessageRequestModel> requests, CancellationToken cancellationToken);
}
