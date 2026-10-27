namespace WorkerQuartzInteraction.Domain.Interfaces;

public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
