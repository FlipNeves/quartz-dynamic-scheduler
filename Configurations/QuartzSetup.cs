using Quartz;
using WorkerQuartzInteraction.Domain.Interfaces;
using WorkerQuartzInteraction.Infrastructure.Broker;
using WorkerQuartzInteraction.Infrastructure.Repositories;
using WorkerQuartzInteraction.Infrastructure.Scheduler;
using WorkerQuartzInteraction.Jobs;

namespace WorkerQuartzInteraction.Configurations
{
    public static class QuartzSetup
    {
        public static void AddQuartzScheduler(this IServiceCollection services, IConfiguration configuration)
        {
            // Pega as cron expressions do appsettings.json (para jobs estáticos)
            var cronSampleJob = configuration.GetValue<string>("QuartzJobs:SampleJob:Cron") ?? "0/30 * * * * ?";

            services.AddQuartz(q =>
            {
                // Static Sample Job Configuration
                var jobKey = new JobKey("StaticSampleJob");
                q.AddJob<QuartzJobAdapter<SampleQuartzJob>>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("StaticSampleJob-trigger")
                    .WithCronSchedule(cronSampleJob));
            });

            // Standard Quartz Hosting
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // 1. Dynamic Scheduling Infrastructure
            services.AddSingleton<JobSchedulerService>();
            services.AddHostedService<JobInitializerService>();
            
            // 2. Repository Mapping
            services.AddScoped<IJobConfigurationRepository, JobConfigurationRepository>();

            // 3. Broker Implementation
            services.AddSingleton<IEventPublisher, EventPublisher>();

            // 4. Register Domain Jobs for DI
            services.AddScoped<SampleQuartzJob>();
        }
    }
}
