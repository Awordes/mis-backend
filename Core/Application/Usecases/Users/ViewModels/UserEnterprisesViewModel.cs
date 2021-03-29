using System.Collections.Generic;
using Core.Application.Usecases.Enterprises.ViewModels;

namespace Core.Application.Usecases.Users.ViewModels
{
    public class UserEnterprisesViewModel
    {
        public ICollection<EnterpriseViewModel> Enterprises { get; set; }
    }
}