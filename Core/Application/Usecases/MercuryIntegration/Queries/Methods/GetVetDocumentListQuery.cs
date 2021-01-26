using Core.Application.Common;
using Core.Application.Usecases.MercuryIntegration.Queries.Requests;
using Core.Domain.MercuryModels.Common;
using Core.Domain.MercuryModels.Data;
using Core.Domain.MercuryModels.Methods;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Methods
{
    public class GetVetDocumentListQuery : IRequest<object>
    {
        /// <summary>
        /// Количество ВСД
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Номер первого ВСД
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Статус ВСД
        /// </summary>
        public VetDocumentStatus Status { get; set; }

        /// <summary>
        /// Тип ВСД
        /// </summary>
        public VetDocumentType Type { get; set; }

        private class Handler : IRequestHandler<GetVetDocumentListQuery, object>
        {
            private readonly MercuryConstants _mercuryConstantsOption;

            private readonly IMediator _mediator;

            public Handler(IMediator mediator, IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _mediator = mediator;
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
            }

            public async Task<object> Handle(GetVetDocumentListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var data = new GetVetDocumentListData
                    {
                        getVetDocumentListRequest = new GetVetDocumentListRequest
                        {
                            localTransactionId = _mercuryConstantsOption.LocalTransactionId,
                            initiator = new Initiator
                            {
                                login = _mercuryConstantsOption.InitiatorLogin
                            },
                            listOptions = new ListOptions
                            {
                                count = request.Count,
                                offset = request.Offset
                            },
                            vetDocumentStatus = request.Status,
                            vetDocumentType = request.Type,
                            enterpriseGuid = _mercuryConstantsOption.EnterpriseId
                        }
                    };

                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("dt", "http://api.vetrf.ru/schema/cdm/dictionary/v2");
                    namespaces.Add("bs", "http://api.vetrf.ru/schema/cdm/base");
                    namespaces.Add("vd", "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2");
                    namespaces.Add("merc", "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2");

                    var result = await _mediator.Send(new SubmitRequestQuery
                    {
                        Data = data,
                        Namespaces = namespaces
                    });

                    return result;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
