using Quartz;
using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Infrastructure.Scheduler
{
    [DisallowConcurrentExecution]
    public class QuartzJobAdapter<TJob> : Quartz.IJob where TJob : Domain.Interfaces.IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QuartzJobAdapter<TJob>> _logger;

        public QuartzJobAdapter(IServiceProvider serviceProvider, ILogger<QuartzJobAdapter<TJob>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobName = typeof(TJob).Name;
            
            using var scope = _serviceProvider.CreateScope();
            var jobImplementation = scope.ServiceProvider.GetRequiredService<TJob>();

            _logger.LogInformation("[Quartz] Iniciando execução do Job: {JobName} às {Time}", jobName, DateTimeOffset.Now);
            
            try
            {
                await jobImplementation.ExecuteAsync(context.CancellationToken);
                _logger.LogInformation("[Quartz] Job {JobName} finalizado com sucesso às {Time}", jobName, DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Quartz] Erro durante a execução do Job {JobName}", jobName);
                throw new JobExecutionException(msg: ex.Message, cause: ex, refireImmediately: false);
            }
        }
    }
}
