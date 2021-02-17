using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Domain.Mercury;
using MediatR;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class GetVsdStatusesQuery: IRequest<object>
    {
        private class Handler : IRequestHandler<GetVsdStatusesQuery, object>
        {
            private readonly IMercuryService _mercuryService;

            public Handler(IMercuryService mercuryService)
            {
                _mercuryService = mercuryService;
            }
            
            public async Task<object> Handle(GetVsdStatusesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await Task.FromResult(_mercuryService.GetVsdStatusListViewModel());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}