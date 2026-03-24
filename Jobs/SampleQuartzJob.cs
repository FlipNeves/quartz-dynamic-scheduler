using WorkerQuartzInteraction.Domain.Interfaces;

namespace WorkerQuartzInteraction.Jobs
{
    /// <summary>
    /// Demonstrates a job that performs business logic and publishes a result to RabbitMQ.
    /// This job is mapped to the "SampleDynamicJob" ID in our mock database.
    /// </summary>
    public class SampleQuartzJob : IJob
    {
        private readonly ILogger<SampleQuartzJob> _logger;
        private readonly IEventPublisher _publisher;

        public string JobId => "SampleQuartzJob";

        public SampleQuartzJob(ILogger<SampleQuartzJob> logger, IEventPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[{JobId}] Processing business rules...", JobId);

            // Simulate some complex calculation or data retrieval
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            var result = new { Timestamp = DateTime.Now, Status = "Success", Source = JobId };

            // Publish the result to a message broker for other services to consume
            await _publisher.PublishAsync(result, "job-results-queue", cancellationToken);

            _logger.LogInformation("[{JobId}] Job completed and event published.", JobId);
        }
    }
}
