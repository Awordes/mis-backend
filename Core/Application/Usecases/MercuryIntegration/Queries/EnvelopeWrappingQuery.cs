using Core.Application.Common;
using Core.Domain.MercuryModels;
using Core.Domain.MercuryModels.Bodies;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class EnvelopeWrappingQuery : IQuery, IRequest<object>
    {
        public BodyAbstract Body { get; set; }

        public XmlSerializerNamespaces Namespaces { get; set; }

        public class Handler : IRequestHandler<EnvelopeWrappingQuery, object>
        {
            private readonly MercuryConstants _mercuryConstantsOption;

            private readonly IMediator _mediator;

            public Handler(IMediator mediator, IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _mediator = mediator;
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
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

                        new XmlSerializer(envelope.GetType(), root)
                            .Serialize(writer, envelope, request.Namespaces);
                    }

                    return await _mediator.Send(new SendApplicationRequestQuery { RequestBody = requestBody });
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
