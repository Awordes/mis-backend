using System;
using System.Collections.Generic;
using Core.Application.Common.Services;
using Core.Domain.Auth;

namespace Infrastructure.Services
{
    public class AutoVsdProcessDataService: IAutoVsdProcessDataService
    {
        public DateTime AutoProcessEnd { get; set; }

        public object Locker { get; set; } = new();

        public ICollection<User> Users { get; set; }
    }
}