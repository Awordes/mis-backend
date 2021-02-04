using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Mercury;
using MediatR;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Methods
{
    public class GetVsdStatusesQuery: IRequest<object>
    {
        private class Handler : IRequestHandler<GetVsdStatusesQuery, object>
        {
            public async Task<object> Handle(GetVsdStatusesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await Task.FromResult(VsdStatus.GetAll());
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