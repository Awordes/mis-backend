namespace Infrastructure.Options
{
    public class IdentityConfigurationOptions
    {
        public const string SectionName = nameof(IdentityConfigurationOptions);
        
        public Password Password { get; set; }

        public Cookie Cookie { get; set; }
    }

    public class Password
    {
        public int RequiredLength { get; set; }
        
        public bool RequireLowercase { get; set; }
        
        public bool RequireUppercase { get; set; }
        
        public bool RequireNonAlphanumeric { get; set; }
        
        public bool RequireDigit { get; set; }
    }

    public class Cookie
    {
        public bool HttpOnly { get; set; }
        
        public bool SlidingExpiration { get; set; }
        
        public int ExpireTimeSpanHoursCount { get; set; }
        
        public string Name { get; set; }
    }
}