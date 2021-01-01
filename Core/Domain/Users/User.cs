using Microsoft.AspNetCore.Identity;
using System;

namespace Core.Domain.Users
{
    public class User: IdentityUser<Guid>
    {
    }
}
