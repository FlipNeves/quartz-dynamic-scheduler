using Quartz;
using WorkerQuartzInteraction.Domain.Entities;
using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Infrastructure.Scheduler
{
    /// <summary>
    /// This service manages Quartz.NET jobs at runtime.
    /// It allows scheduling, updating, and removing jobs dynamically based on their database configurations.
    /// </summary>
    public class JobSchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<JobSchedulerService> _logger;

        public JobSchedulerService(ISchedulerFactory schedulerFactory, ILogger<JobSchedulerService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        /// <summary>
        /// Registers or updates a job schedule in the Quartz scheduler.
        /// </summary>
        /// <typeparam name="TJob">The job implementation type (Domain Job).</typeparam>
        public async Task ScheduleJobAsync<TJob>(JobConfiguration config, CancellationToken cancellationToken = default) 
            where TJob : Domain.Interfaces.IJob
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            var jobKey = new JobKey(config.JobId, "DynamicGroup");
            var triggerKey = new TriggerKey($"{config.JobId}-trigger", "DynamicGroup");

            // We use our Adapter to wrap the domain job
            var jobDetail = JobBuilder.Create<QuartzJobAdapter<TJob>>()
                .WithIdentity(jobKey)
                .WithDescription(config.JobName)
                .Build();

            // Create the trigger with the CRON expression from the database
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule(config.CronExpression)
                .StartNow()
                .Build();

            // Check if the job already exists to update it
            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                _logger.LogInformation("Updating existing job: {JobId}", config.JobId);
                await scheduler.RescheduleJob(triggerKey, trigger, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Scheduling new job: {JobId}", config.JobId);
                await scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
            }

            // If the job should be inactive, we pause it immediately
            if (!config.IsActive)
            {
                await scheduler.PauseJob(jobKey, cancellationToken);
                _logger.LogWarning("Job {JobId} is scheduled but currently PAUSED.", config.JobId);
            }
        }

        /// <summary>
        /// Removes a job from the scheduler.
        /// </summary>
        public async Task RemoveJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            var jobKey = new JobKey(jobId, "DynamicGroup");

            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                await scheduler.DeleteJob(jobKey, cancellationToken);
                _logger.LogInformation("Job {JobId} removed successfully.", jobId);
            }
        }
    }
}
