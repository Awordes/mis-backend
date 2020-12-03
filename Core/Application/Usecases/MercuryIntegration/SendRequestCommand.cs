using Core.Application.Common;
using Core.Application.Usecases.MercuryIntegration.SendModels;
using Core.Application.Usecases.MercuryIntegration.SendModels.Data;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration
{
    public class SendRequestCommand: ICommand, IRequest
    {
        public class Handler : IRequestHandler<SendRequestCommand>
        {
            private readonly IMisDbContext _context;

            private readonly MercuryConstants _mercuryConstantsOption;

            public Handler(IMisDbContext context, IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _context = context;
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
            }

            public async Task<Unit> Handle(SendRequestCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var envelope = new Envelope
                    {
                        Header = "",
                        Body = new Body
                        {
                            submitApplicationRequest = new SubmitApplicationRequest
                            {
                                apiKey = _mercuryConstantsOption.ApiKey,
                                application = new SendModels.Application
                                {
                                    serviceId = _mercuryConstantsOption.ServiceId,
                                    issuerId = _mercuryConstantsOption.IssuerId,
                                    issueDate = DateTime.Now,
                                    data = new GetVetDocumentListData
                                    {
                                        getVetDocumentListRequest = new SendModels.Requests.GetVetDocumentListRequest
                                        {
                                            localTransactionId = _mercuryConstantsOption.LocalTransactionId,
                                            initiator = new SendModels.CommonModels.Initiator
                                            {
                                                login = _mercuryConstantsOption.InitiatorLogin
                                            },
                                            listOptions = new SendModels.CommonModels.ListOptions
                                            {
                                                count = 10
                                            },
                                            vetDocumentType = SendModels.CommonModels.VetDocumentType.INCOMING,
                                            vetDocumentStatus = SendModels.CommonModels.VetDocumentStatus.WITHDRAWN,
                                            enterpriseGuid = _mercuryConstantsOption.EnterpriseId
                                        }
                                    }
                                }
                            }
                        }
                    };

                    var doc = new XmlDocument();

                    using (var writer = doc.CreateNavigator().AppendChild())
                    {
                        var root = new XmlRootAttribute
                        {
                            ElementName = "Envelope",
                            Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
                        };

                        var ns = new XmlSerializerNamespaces();
                        ns.Add("dt", "http://api.vetrf.ru/schema/cdm/dictionary/v2");
                        ns.Add("bs", "http://api.vetrf.ru/schema/cdm/base");
                        ns.Add("merc", "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2");
                        ns.Add("apldef", "http://api.vetrf.ru/schema/cdm/application/ws-definitions");
                        ns.Add("apl", "http://api.vetrf.ru/schema/cdm/application");
                        ns.Add("vd", "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2");
                        ns.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");

                        new XmlSerializer(envelope.GetType(), root)
                            .Serialize(writer, envelope, ns);
                    }

                    return Unit.Value;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
