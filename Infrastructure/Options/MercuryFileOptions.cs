namespace Infrastructure.Options
{
    public class MercuryFileOptions
    {
        public const string SectionName = nameof(MercuryFileOptions);
        
        public string Folder { get; set; }
    }
}