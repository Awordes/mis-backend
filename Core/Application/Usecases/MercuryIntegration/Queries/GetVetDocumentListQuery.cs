using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using MediatR;
using Microsoft.Extensions.Options;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class GetVetDocumentListQuery : IRequest<VsdListViewModel>
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы
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

        private class Handler : IRequestHandler<GetVetDocumentListQuery, VsdListViewModel>
        {
            private readonly MercuryOptions _mercuryOptions;
            private readonly IMercuryService _mercuryService;

            public Handler(
                IOptionsMonitor<MercuryOptions> mercuryOptions,
                IMercuryService mercuryService)
            {
                _mercuryOptions = mercuryOptions.CurrentValue;
                _mercuryService = mercuryService;
            }

            public async Task<VsdListViewModel> Handle(GetVetDocumentListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var count = request.PageSize;

                    var offset = request.PageSize * (request.Page - 1);

                    var vm =  await _mercuryService.GetVetDocumentList(
                        _mercuryOptions.LocalTransactionId,
                        _mercuryOptions.InitiatorLogin,
                        count,
                        offset,
                        request.Type,
                        request.Status,
                        _mercuryOptions.EnterpriseId
                    );

                    vm.PageSize = request.PageSize;
                    vm.CurrentPage = request.Page;
                    vm.PageCount = vm.ElementCount / vm.PageSize + (vm.ElementCount % vm.PageSize > 0 ? 1 : 0);
                    
                    return vm;
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
