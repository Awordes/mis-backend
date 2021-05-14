using System;

namespace Core.Domain
{
    public class MercuryFileInfo
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }
    }
}