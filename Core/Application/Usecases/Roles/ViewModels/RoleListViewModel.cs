using System.Collections.Generic;

namespace Core.Application.Usecases.Roles.ViewModels
{
    public class RoleListViewModel
    {
        public ICollection<RoleViewModel> Roles { get; set; }
    }
}