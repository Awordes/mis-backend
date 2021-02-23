using System;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;

namespace Core.Application.Usecases.Enterprises.ViewModels
{
    public class EnterpriseViewModel: IMapFrom<Enterprise>
    {
        public Guid Id { get; set; }

        public string MercuryId { get; set; }

        public string Name { get; set; }
    }
}