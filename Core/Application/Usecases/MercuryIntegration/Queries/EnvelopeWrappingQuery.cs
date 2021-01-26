using Core.Domain.MercuryModels;
using Core.Domain.MercuryModels.Bodies;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class EnvelopeWrappingQuery : IRequest<object>
    {
        public BodyAbstract Body { get; set; }

        public XmlSerializerNamespaces Namespaces { get; set; }

        public string AbstractBodyName { get; set; }

        private class Handler : IRequestHandler<EnvelopeWrappingQuery, object>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<object> Handle(EnvelopeWrappingQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var envelope = new Envelope
                    {
                        Header = "",
                        Body = request.Body
                    };

                    var requestBody = new XmlDocument();

                    using (var writer = requestBody.CreateNavigator().AppendChild())
                    {
                        var root = new XmlRootAttribute
                        {
                            ElementName = "Envelope",
                            Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
                        };

                        request.Namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

                        new XmlSerializer(typeof(Envelope), root)
                            .Serialize(writer, envelope, request.Namespaces);
                    }

                    XmlDocument xmlResponse =  await _mediator.Send(new SendApplicationRequestQuery { RequestBody = requestBody });

                    var attr1 = xmlResponse.CreateAttribute("xmlns:p8");
                    attr1.Value = "http://www.w3.org/2001/XMLSchema-instance";
                    xmlResponse.FirstChild.Attributes.Append(attr1);

                    var attr2 = xmlResponse.CreateAttribute("p8", "type", "http://www.w3.org/2001/XMLSchema-instance");
                    attr2.Value = $"soap:{request.AbstractBodyName}";
                    xmlResponse.FirstChild.FirstChild.Attributes.Append(attr2);

                    var rooot = new XmlRootAttribute
                    {
                        ElementName = "Envelope",
                        Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
                    };

                    var response = new XmlSerializer(typeof(Envelope), rooot).Deserialize(new XmlNodeReader(xmlResponse)) as Envelope;

                    return new 
                    {
                        applicationId = (response.Body as SubmitResponseBody)
                            .submitApplicationResponse.application.applicationId,

                        status = (response.Body as SubmitResponseBody)
                            .submitApplicationResponse.application.status.ToString()
                    };
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
