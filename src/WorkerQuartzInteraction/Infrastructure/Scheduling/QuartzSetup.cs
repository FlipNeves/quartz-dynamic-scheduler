using Quartz;
using WorkerQuartzInteraction.Configurations;
using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Infrastructure.Scheduling;

public static class QuartzSetup
{
    public static IServiceCollection AddQuartzScheduler(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(SchedulerOptions.Section).Get<SchedulerOptions>() ?? new SchedulerOptions();

        services.AddQuartz(quartz => quartz.UseDefaultThreadPool(pool => pool.MaxConcurrency = options.WorkerCount));
        services.AddQuartzHostedService(hosting => hosting.WaitForJobsToComplete = true);

        services.AddSingleton<IJobScheduler, QuartzJobScheduler>();
        return services;
    }
}
