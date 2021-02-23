using System;

namespace Core.Domain.Auth
{
    public class Enterprise
    {
        public Guid Id { get; set; }

        public string MercuryId { get; set; }

        public string Name { get; set; }

        public User User { get; set; }
    }
}