using Microsoft.AspNetCore.Identity;
using System;

namespace Core.Domain.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string Inn { get; set; }

        public string Title { get; set; }

        public string Contact { get; set; }

        public string MercuryLogin { get; set; }

        public string MercuryPassword { get; set; }

        public string ApiLogin { get; set; }

        public string ApiPassword { get; set; }

        public string ApiKey { get; set; }

        public string IssuerId { get; set; }

        public bool EditAllow { get; set; }

        public bool Deleted { get; set; }
    }
}
