using System;

namespace Core.Domain.Operations
{
    public class VsdProcessTransaction
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; init; } = DateTime.Now;

        public DateTime FinishTime { get; set; }

        public string VsdId { get; set; }

        public Operation Operation { get; set; }

        public string Error { get; set; }
    }
}