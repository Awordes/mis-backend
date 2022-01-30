namespace Infrastructure.Options
{
    public class LogFolderOptions
    {
        public const string SectionName = nameof(LogFolderOptions);

        public bool StoreLogs { get; set; }
        
        public string Folder { get; set; }
    }
}