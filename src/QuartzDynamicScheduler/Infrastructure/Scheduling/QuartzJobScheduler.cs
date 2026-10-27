using Quartz;
using QuartzDynamicScheduler.Domain.Entities;
using QuartzDynamicScheduler.Domain.Interfaces;

namespace QuartzDynamicScheduler.Infrastructure.Scheduling;

public sealed class QuartzJobScheduler : IJobScheduler
{
    private const string Group = "jobs";

    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzJobScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public bool IsValidCron(string cronExpression) => CronExpression.TryParse(cronExpression, CronFormat.Unix, out _);

    public async Task ScheduleAsync(JobConfiguration configuration, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKey = new TriggerKey(configuration.JobId, Group);
        var cron = CronExpression.Parse(configuration.CronExpression, CronFormat.Unix);

        if (await scheduler.GetTrigger(triggerKey, cancellationToken) is ICronTrigger current
            && current.CronExpressionString == cron.CronExpressionString)
            return;

        var job = JobBuilder.Create<QuartzJobRunner>()
            .WithIdentity(configuration.JobId, Group)
            .UsingJobData(QuartzJobRunner.JobIdKey, configuration.JobId)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .WithSchedule(CronScheduleBuilder.Create(cron).InTimeZone(TimeZoneInfo.Utc))
            .Build();

        await scheduler.ScheduleJob(job, [trigger], ScheduleJobOptions.Replacing, cancellationToken);
    }

    public async Task UnscheduleAsync(string jobId, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.DeleteJob(new JobKey(jobId, Group), cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetScheduledJobIdsAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(Group), cancellationToken);
        return jobKeys.Select(jobKey => jobKey.Name).ToList();
    }

    public async Task<DateTime?> GetNextRunAsync(string jobId, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var trigger = await scheduler.GetTrigger(new TriggerKey(jobId, Group), cancellationToken);
        return trigger?.NextFireTimeUtc?.UtcDateTime;
    }
}
