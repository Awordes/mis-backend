using Core.Domain.MercuryModels;
using Core.Domain.MercuryModels.Bodies;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class EnvelopeWrappingQuery : IRequest<XmlDocument>
    {
        public BodyAbstract Body { get; set; }

        public XmlSerializerNamespaces Namespaces { get; set; }

        private class Handler : IRequestHandler<EnvelopeWrappingQuery, XmlDocument>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<XmlDocument> Handle(EnvelopeWrappingQuery request, CancellationToken cancellationToken)
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

                    return  await _mediator.Send(new SendApplicationRequestQuery { RequestBody = requestBody });
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
