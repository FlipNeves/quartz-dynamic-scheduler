using Quartz;
using QuartzDynamicScheduler.Configurations;
using QuartzDynamicScheduler.Domain.Interfaces;

namespace QuartzDynamicScheduler.Infrastructure.Scheduling;

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
