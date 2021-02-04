using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Mercury;
using MediatR;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Methods
{
    public class GetVsdTypesQuery: IRequest<object>
    {
        private class Handler : IRequestHandler<GetVsdTypesQuery, object>
        {
            public async Task<object> Handle(GetVsdTypesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    return await Task.FromResult(VsdType.GetAll());
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