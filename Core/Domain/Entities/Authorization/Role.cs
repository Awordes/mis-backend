using System;

namespace Core.Domain.Entities.Authorization
{
    public class Role: IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SysName { get; set; }
    }
}
