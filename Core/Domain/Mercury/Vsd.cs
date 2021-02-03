using System;

namespace Core.Domain.Mercury
{
    public class Vsd
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime ProductDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public decimal Volume { get; set; }

        public string ProductGlobalId { get; set; }
    }
}