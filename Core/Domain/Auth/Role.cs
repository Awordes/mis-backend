using Microsoft.AspNetCore.Identity;
using System;

namespace Core.Domain.Auth
{
    public class Role : IdentityRole<Guid>
    {
        public string Description { get; set; }
        
        public Role(string name) : base(name) {}

        public Role(string name, string description) : base(name)
        {
            Description = description;
        }
    }
}
