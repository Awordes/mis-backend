using System;

namespace Core.Domain.Entities.Authorization
{
    class Account: IEntity
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public string UserName { get; set; }

        public Role Role { get; set; }
    }
}
