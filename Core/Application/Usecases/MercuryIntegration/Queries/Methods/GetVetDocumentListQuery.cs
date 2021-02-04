using Core.Application.Common;
using Core.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Methods
{
    public class GetVetDocumentListQuery : IRequest<object>
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Кол-во элементов на странице
        /// </summary>
        public int PageSize { get; set; }

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
                    var count = request.PageSize;

                    var offset = request.PageSize * (request.Page - 1);

                    return await _mercuryService.GetVetDocumentList(
                        _mercuryOptions.LocalTransactionId,
                        _mercuryOptions.InitiatorLogin,
                        count,
                        offset,
                        request.Type,
                        request.Status,
                        _mercuryOptions.EnterpriseId
                    );
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
