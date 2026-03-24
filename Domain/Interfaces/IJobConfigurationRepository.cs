using WorkerQuartzInteraction.Domain.Entities;

namespace WorkerQuartzInteraction.Domain.Interfaces
{
    /// <summary>
    /// Repository interface to manage job configurations.
    /// In a production environment, this would be implemented with EF Core or Dapper.
    /// </summary>
    public interface IJobConfigurationRepository
    {
        Task<IEnumerable<JobConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<JobConfiguration?> GetByIdAsync(string jobId, CancellationToken cancellationToken = default);
        Task UpdateAsync(JobConfiguration configuration, CancellationToken cancellationToken = default);
    }
}
