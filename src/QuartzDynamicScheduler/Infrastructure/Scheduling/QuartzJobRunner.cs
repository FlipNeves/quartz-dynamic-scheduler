using Quartz;
using QuartzDynamicScheduler.Services;

namespace QuartzDynamicScheduler.Infrastructure.Scheduling;

public sealed class QuartzJobRunner : IJob
{
    public const string JobIdKey = "jobId";

    private readonly JobRunner _runner;

    public QuartzJobRunner(JobRunner runner)
    {
        _runner = runner;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var jobId = context.MergedJobDataMap.GetString(JobIdKey)
            ?? throw new JobExecutionException($"The job data has no '{JobIdKey}' entry.");

        try
        {
            await _runner.RunAsync(jobId, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new JobExecutionException(exception);
        }
    }
}
