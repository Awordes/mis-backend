using System;
using System.Collections.Generic;
using Core.Domain.Auth;

namespace Core.Application.Common.Services
{
    public interface IAutoVsdProcessDataService
    {
        public DateTime AutoProcessEnd { get; set; }

        public object Locker { get; set; }

        public ICollection<User> Users { get; set; }
        
        public Dictionary<Guid, ICollection<string>> VsdBlackList { get; set; }
    }
}