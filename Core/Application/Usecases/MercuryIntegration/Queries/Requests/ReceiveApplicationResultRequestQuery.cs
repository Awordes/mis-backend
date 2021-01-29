using System.Xml.Linq;
using Core.Application.Common;
using Core.Domain.MercuryModels;
using Core.Domain.MercuryModels.Bodies;
using Core.Domain.MercuryModels.Requests;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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

                    var xmlResponse =  await _mediator.Send(new EnvelopeWrappingQuery
                    {
                        Body = recieveResultRequestBody,
                        Namespaces = namespaces
                    });

                    if (xmlResponse.FirstChild.GetType() == typeof(XmlDeclaration))
                    {
                        xmlResponse.RemoveChild(xmlResponse.FirstChild);
                    }
                    
                    var attr1 = xmlResponse.CreateAttribute("xmlns:p8");
                    attr1.Value = "http://www.w3.org/2001/XMLSchema-instance";
                    xmlResponse.FirstChild.Attributes.Append(attr1);
                    
                    var attr2 = xmlResponse.CreateAttribute("p8", "type", "http://www.w3.org/2001/XMLSchema-instance");
                    attr2.Value = $"soap:{nameof(RecieveResultResponseBody)}";
                    xmlResponse.FirstChild.FirstChild.Attributes.Append(attr2);

                    var root = new XmlRootAttribute
                    {
                        ElementName = "Envelope",
                        Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
                    };

                    var response = new XmlSerializer(typeof(Envelope), root)
                        .Deserialize(new XmlNodeReader(xmlResponse)) as Envelope;

                    return (response.Body as RecieveResultResponseBody).receiveApplicationResultResponse.application ;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
