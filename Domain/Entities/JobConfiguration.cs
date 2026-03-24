namespace WorkerQuartzInteraction.Domain.Entities
{
    /// <summary>
    /// Represents the configuration for a background job.
    /// This entity is typically stored in a database to allow dynamic scheduling.
    /// </summary>
    public class JobConfiguration
    {
        public string JobId { get; set; } = string.Empty;
        public string JobName { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? JobDataJson { get; set; }
    }
}
