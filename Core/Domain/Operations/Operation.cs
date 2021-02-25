using System;
using Core.Domain.Auth;

namespace Core.Domain.Operations
{
    public class Operation
    {
        public Guid Id { get; set; }
        
        public User User { get; set; }

        public DateTime StartTime { get; init; } = DateTime.Now;

        public DateTime FinishTime { get; set; }

        public OperationType Type { get; set; }
    }
}