namespace Infrastructure.Options
{
    public class MisDbOptions
    {
        public const string SectionName = nameof(MisDbOptions);
        
        public string ConnectionString { get; set; }
        
        public string SchemaName { get; set; }
        
        public string EfMigrationsHistoryTableName { get; set; }
        
        public PostgreSqlVersion PostgreSqlVersion { get; set; }
    }

    public class PostgreSqlVersion
    {
        public int Major { get; set; }

        public int Minor { get; set; }
    }
}