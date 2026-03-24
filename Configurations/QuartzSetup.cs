using Quartz;
using WorkerQuartzInteraction.Infrastructure.Scheduler;
using WorkerQuartzInteraction.Jobs;

namespace WorkerQuartzInteraction.Configurations
{
    public static class QuartzSetup
    {
        public static void AddQuartzScheduler(this IServiceCollection services, IConfiguration configuration)
        {
            // Pega as cron expressions do appsettings.json
            var cronSampleJob = configuration.GetValue<string>("QuartzJobs:SampleJob:Cron") ?? "0/30 * * * * ?"; // Padrão: a cada 30 segundos

            services.AddQuartz(q =>
            {
                // Configuração base (opcional: persistência em BD, JSON serializer, etc.)
                
                // Registra o SampleJob
                var jobKey = new JobKey(nameof(SampleQuartzJob));
                q.AddJob<QuartzJobAdapter<SampleQuartzJob>>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{nameof(SampleQuartzJob)}-trigger")
                    .WithCronSchedule(cronSampleJob));
            });

            // Quartz.Extensions.Hosting hospeda o Quartz como um IHostedService nativo
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // Registro dos Jobs de negócio no DI
            services.AddScoped<SampleQuartzJob>();
        }
    }
}
