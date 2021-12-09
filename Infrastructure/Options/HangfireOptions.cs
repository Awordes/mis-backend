namespace Infrastructure.Options
{
    public class HangfireOptions
    {
        public const string SectionName = nameof(HangfireOptions);

        public string SchemaName { get; set; }

        public int WorkerCount { get; set; }

        public string MisServerName { get; set; }

        public int HeartbeatIntervalMinutesCount { get; set; }
    }
}