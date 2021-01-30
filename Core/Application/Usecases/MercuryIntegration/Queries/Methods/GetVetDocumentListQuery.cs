using Core.Application.Common;
using Core.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Methods
{
    public class GetVetDocumentListQuery : IRequest<object>
    {
        /// <summary>
        /// Количество ВСД
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Номер первого ВСД
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Статус ВСД
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Тип ВСД
        /// </summary>
        public int Type { get; set; }

        private class Handler : IRequestHandler<GetVetDocumentListQuery, object>
        {
            private readonly MercuryOptions _mercuryOptions;
            private readonly IMediator _mediator;
            private readonly IMercuryService _mercuryService;

            public Handler(
                IMediator mediator,
                IOptionsMonitor<MercuryOptions> mercuryOptions,
                IMercuryService mercuryService)
            {
                _mediator = mediator;
                _mercuryOptions = mercuryOptions.CurrentValue;
                _mercuryService = mercuryService;
            }

            public async Task<object> Handle(GetVetDocumentListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await _mercuryService.GetVetDocumentList(_mercuryOptions.LocalTransactionId,
                    _mercuryOptions.InitiatorLogin, request.Count, request.Offset, request.Type, request.Status, _mercuryOptions.EnterpriseId);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
