namespace Infrastructure.Options
{
    public class AutoVsdProcessingOptions
    {
        public const string SectionName = nameof(AutoVsdProcessingOptions);

        public QuartzOptions QuartzOptions { get; set; }

        public AutoVsdProcessingTimeSpan AutoVsdProcessingTimeSpan { get; set; }
    }

    public class QuartzOptions
    {
        public string SchedulerId { get; set; }

        public int ThreadPoolMaxConcurrency { get; set; }

        public string JobTriggerIdentity { get; set; }

        public string CronSchedule { get; set; }
    }

    public class AutoVsdProcessingTimeSpan
    {
        public int Seconds { get; set; }

        public int Minutes { get; set; }

        public int Hours { get; set; }
    }
}