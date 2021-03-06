using System.Collections.Generic;

namespace Core.Application.Usecases.Users.ViewModels
{
    public class UserListViewModel
    {
        public ICollection<UserViewModel> Users { get; set; }
    }
}