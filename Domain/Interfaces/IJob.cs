namespace WorkerQuartzInteraction.Domain.Interfaces
{
    public interface IJob
    {
        string JobId { get; }
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
