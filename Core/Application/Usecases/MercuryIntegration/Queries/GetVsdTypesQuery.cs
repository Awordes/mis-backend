using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MediatR;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class GetVsdTypesQuery: IRequest<object>
    {
        private class Handler : IRequestHandler<GetVsdTypesQuery, object>
        {
            private readonly IMercuryService _mercuryService;

            public Handler(IMercuryService mercuryService)
            {
                _mercuryService = mercuryService;
            }
            
            public async Task<object> Handle(GetVsdTypesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await Task.FromResult(_mercuryService.GetVsdTypeListViewModel());
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