using WorkerQuartzInteraction.Domain.Entities;
using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Infrastructure.Repositories
{
    /// <summary>
    /// Mock implementation of the IJobConfigurationRepository.
    /// In a real project, this would fetch data from SQL Server, PostgreSQL, etc.
    /// </summary>
    public class JobConfigurationRepository : IJobConfigurationRepository
    {
        private static readonly List<JobConfiguration> _mockData = new()
        {
            new JobConfiguration
            {
                JobId = "SampleDynamicJob",
                JobName = "A Job Scheduled via Database",
                CronExpression = "0/20 * * * * ?", // Runs every 20 seconds
                IsActive = true
            },
            new JobConfiguration
            {
                JobId = "AnotherSampleJob",
                JobName = "A second Job from DB",
                CronExpression = "0 0/5 * * * ?", // Runs every 5 minutes
                IsActive = false // Starts paused
            }
        };

        public Task<IEnumerable<JobConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<JobConfiguration>>(_mockData);
        }

        public Task<JobConfiguration?> GetByIdAsync(string jobId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_mockData.FirstOrDefault(j => j.JobId == jobId));
        }

        public Task UpdateAsync(JobConfiguration configuration, CancellationToken cancellationToken = default)
        {
            var existing = _mockData.FirstOrDefault(j => j.JobId == configuration.JobId);
            if (existing != null)
            {
                existing.CronExpression = configuration.CronExpression;
                existing.IsActive = configuration.IsActive;
            }
            return Task.CompletedTask;
        }
    }
}
