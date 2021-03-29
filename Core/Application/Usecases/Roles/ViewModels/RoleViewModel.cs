using System;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;

namespace Core.Application.Usecases.Roles.ViewModels
{
    public class RoleViewModel: IMapFrom<Role>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}