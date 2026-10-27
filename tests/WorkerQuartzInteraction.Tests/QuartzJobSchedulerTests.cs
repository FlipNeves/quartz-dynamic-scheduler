using Quartz;
using WorkerQuartzInteraction.Domain.Entities;
using WorkerQuartzInteraction.Infrastructure.Scheduling;

namespace WorkerQuartzInteraction.Tests;

public sealed class QuartzJobSchedulerTests : IAsyncLifetime
{
    private readonly StandaloneSchedulerFactory _schedulerFactory = QuartzSchedulerBuilder
        .Create(quartz => quartz.ConfigureScheduler(scheduler => scheduler.InstanceName = Guid.NewGuid().ToString()))
        .Build();

    private readonly QuartzJobScheduler _scheduler;

    public QuartzJobSchedulerTests()
    {
        _scheduler = new QuartzJobScheduler(_schedulerFactory);
    }

    [Fact]
    public async Task ScheduleAsync_RegistersTheJobWithItsNextRun()
    {
        await _scheduler.ScheduleAsync(new JobConfiguration { JobId = "SampleJob", CronExpression = "*/5 * * * *" }, CancellationToken.None);

        Assert.Equal("SampleJob", Assert.Single(await _scheduler.GetScheduledJobIdsAsync(CancellationToken.None)));
        var nextRun = await _scheduler.GetNextRunAsync("SampleJob", CancellationToken.None);
        Assert.NotNull(nextRun);
        Assert.Equal(DateTimeKind.Utc, nextRun.Value.Kind);
        Assert.Equal(0, nextRun.Value.Minute % 5);
    }

    [Fact]
    public async Task UnscheduleAsync_RemovesTheJob()
    {
        await _scheduler.ScheduleAsync(new JobConfiguration { JobId = "SampleJob", CronExpression = "* * * * *" }, CancellationToken.None);

        await _scheduler.UnscheduleAsync("SampleJob", CancellationToken.None);

        Assert.Empty(await _scheduler.GetScheduledJobIdsAsync(CancellationToken.None));
        Assert.Null(await _scheduler.GetNextRunAsync("SampleJob", CancellationToken.None));
    }

    [Theory]
    [InlineData("* * * * *", true)]
    [InlineData("*/5 9-18 * * 1-5", true)]
    [InlineData("0 * * ? * *", false)]
    [InlineData("61 * * * *", false)]
    [InlineData("every minute", false)]
    public void IsValidCron_AcceptsTheStandardFiveFieldSyntax(string cronExpression, bool expected) =>
        Assert.Equal(expected, _scheduler.IsValidCron(cronExpression));

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _schedulerFactory.DisposeAsync();
}
