using System.Collections.Generic;

namespace Core.Application.Usecases.Users.ViewModels
{
    public class UserRolesViewModel
    {
        public ICollection<string> Roles { get; set; }
    }
}