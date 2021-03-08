using System;
using Core.Application.Common.Mapping;
using Core.Domain.Operations;

namespace Core.Application.Usecases.Logging.ViewModels
{
    public class VsdProcessTransactionViewModel: IMapFrom<VsdProcessTransaction>
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; init; }

        public DateTime? FinishTime { get; set; }

        public string VsdId { get; set; }

        public string Error { get; set; }
    }
}