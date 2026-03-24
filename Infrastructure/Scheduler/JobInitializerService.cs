using WorkerQuartzInteraction.Domain.Interfaces;
using WorkerQuartzInteraction.Jobs;

namespace WorkerQuartzInteraction.Infrastructure.Scheduler
{
    /// <summary>
    /// This service runs on application startup.
    /// It fetches all job configurations from the repository and schedules them in Quartz.
    /// </summary>
    public class JobInitializerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobInitializerService> _logger;

        public JobInitializerService(IServiceProvider serviceProvider, ILogger<JobInitializerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job Initializer Service is starting...");

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IJobConfigurationRepository>();
            var schedulerService = scope.ServiceProvider.GetRequiredService<JobSchedulerService>();

            var jobConfigs = await repository.GetAllAsync(stoppingToken);

            foreach (var config in jobConfigs)
            {
                // Here we map a JobId from DB to a specific Job Type.
                // In a production app, you might use a Factory or Dictionary mapping strings to Types.
                if (config.JobId == "SampleDynamicJob")
                {
                    await schedulerService.ScheduleJobAsync<SampleQuartzJob>(config, stoppingToken);
                }
                // Add more mappings as needed
            }

            _logger.LogInformation("Dynamic Jobs have been initialized.");
        }
    }
}
