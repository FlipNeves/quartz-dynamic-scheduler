using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Jobs
{
    public class SampleQuartzJob : IJob
    {
        private readonly ILogger<SampleQuartzJob> _logger;

        public string JobId => "SampleQuartzJob";

        public SampleQuartzJob(ILogger<SampleQuartzJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[{JobId}] Iniciando tarefa de processamento...", JobId);

            // Simulação de trabalho
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

            _logger.LogInformation("[{JobId}] Tarefa concluída com sucesso.", JobId);
        }
    }
}
