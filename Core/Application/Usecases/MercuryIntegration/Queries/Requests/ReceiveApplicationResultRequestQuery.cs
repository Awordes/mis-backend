using Core.Application.Common;
using Core.Domain.MercuryModels.Bodies;
using Core.Domain.MercuryModels.Requests;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Requests
{
    public class ReceiveApplicationResultRequestQuery : IRequest<object>
    {
        public string ApplicationId { get; set; }

        private class Handler : IRequestHandler<ReceiveApplicationResultRequestQuery, object>
        {
            private readonly MercuryConstants _mercuryConstantsOption;
            private readonly IMediator _mediator;

            public Handler(IMediator mediator, IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _mediator = mediator;
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
            }

            public async Task<object> Handle(ReceiveApplicationResultRequestQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var recieveResultRequestBody = new RecieveResultRequestBody
                    {
                        receiveApplicationResultRequest = new ReceiveApplicationResultRequest
                        {
                            apiKey = _mercuryConstantsOption.ApiKey,
                            issuerId = _mercuryConstantsOption.IssuerId,
                            applicationId = request.ApplicationId
                        }
                    };

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("apldef", "http://api.vetrf.ru/schema/cdm/application/ws-definitions");

                    return await _mediator.Send(new EnvelopeWrappingQuery
                    {
                        Body = recieveResultRequestBody,
                        Namespaces = namespaces
                    });
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
