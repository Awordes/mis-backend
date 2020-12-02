using System;

namespace Core.Domain.Entities.Authorization
{
    public class User: IEntity
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }
    }
}
