using System;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MercuryAPI;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Core.Application.Usecases.Logging.Commands.VsdProcessTransaction;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Infrastructure.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Integrations.Mercury
{
    public class MercuryService : IMercuryService
    {
        private readonly MercuryOptions _mercuryOptions;
        private readonly IMediator _mediator;

        public MercuryService(IOptionsMonitor<MercuryOptions> mercuryOptions,
            IMediator mediator)
        {
            _mercuryOptions = mercuryOptions.CurrentValue;
            _mediator = mediator;
        }

        public EnumElementListViewModel GetVsdTypeListViewModel()
        {
            return VetDocumentType.INCOMING.GetDisplayNames();
        }

        public EnumElementListViewModel GetVsdStatusListViewModel()
        {
            return VetDocumentStatus.CONFIRMED.GetDisplayNames();
        }
        
        public async Task<VsdListViewModel> GetVetDocumentList(
            string localTransactionId,
            Core.Domain.Auth.User user,
            Core.Domain.Auth.Enterprise enterprise,
            int count,
            int offset,
            int vetDocumentType,
            int vetDocumentStatus
            )
        {
            try
            {
                var requestData = new GetVetDocumentListRequest
                {
                    localTransactionId = localTransactionId,
                    initiator = new User { login = user.MercuryLogin },
                    listOptions = new ListOptions { count = count.ToString(), offset = offset.ToString() },
                    vetDocumentTypeSpecified = true,
                    vetDocumentType = (VetDocumentType)vetDocumentType,
                    vetDocumentStatusSpecified = true,
                    vetDocumentStatus = (VetDocumentStatus)vetDocumentStatus,
                    enterpriseGuid = enterprise.MercuryId
                };

                var result = await requestData.SendRequest<GetVetDocumentListResponse>(
                    user.ApiKey,
                    _mercuryOptions.ServiceId,
                    user.IssuerId,
                    user.ApiLogin,
                    user.ApiPassword
                );

                var vetDocumentList = new List<VsdViewModel>();

                foreach (var vetDocument in result.vetDocumentList.vetDocument)
                {
                    var item = (CertifiedConsignment) vetDocument.Item;
                    
                    var tnns = vetDocument.referencedDocument
                        .Where(x => x.type is DocumentType.Item1 or DocumentType.Item5)
                        .OrderByDescending(x => x.issueDate).ToList();

                    var element = new VsdViewModel
                    {
                        Id = vetDocument.uuid,
                        Name = item.batch.productItem.name,
                        ProductGlobalId = item.batch.productItem.globalID,
                        Volume = item.batch.volume,
                        ProductDate = item.batch.dateOfProduction.firstDate.ToDateTime(),
                        IssueDate = vetDocument.issueDate,
                        ProcessDate = tnns[0]?.issueDate.AddDays(1)
                    }; 

                    vetDocumentList.Add(element);
                }
            
                return new VsdListViewModel
                {
                    result = vetDocumentList,
                    ElementCount = result.vetDocumentList.total
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<object> GetVetDocumentByUuid(
            string localTransactionId,
            Core.Domain.Auth.User user,
            Core.Domain.Auth.Enterprise enterprise,
            string uuid
            )
        {
            try
            {
                var requestData = new GetVetDocumentByUuidRequest
                {
                    localTransactionId = localTransactionId,
                    initiator = new User {login = user.MercuryLogin},
                    enterpriseGuid = enterprise.MercuryId,
                    uuid = uuid
                };

                var result = await requestData.SendRequest<GetVetDocumentByUuidResponse>(
                    user.ApiKey,
                    _mercuryOptions.ServiceId,
                    user.IssuerId,
                    user.ApiLogin,
                    user.ApiPassword
                );

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task ProcessIncomingConsignment(
            string localTransactionId,
            Core.Domain.Auth.User user,
            Core.Domain.Auth.Enterprise enterprise,
            string uuid,
            DateTime? processDate,
            Guid operationId)
        {
            var vsdProcessTransactionId = await _mediator.Send(new VsdProcessTransactionStart
            {
                OperationId = operationId,
                VsdId = uuid
            });
            
            string error = null;

            try
            {
                var vetDocument = (GetVetDocumentByUuidResponse) await GetVetDocumentByUuid(
                    localTransactionId, user, enterprise, uuid);

                var vetDocumentItem = (CertifiedConsignment) vetDocument.vetDocument.Item;

                var batch = vetDocumentItem.batch;

                var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<Batch, Consignment>(); }));

                var consignment = mapper.Map<Consignment>(batch);

                var tnns = vetDocument.vetDocument.referencedDocument
                              .Where(x => x.type is DocumentType.Item1 or DocumentType.Item5)
                              .OrderByDescending(x => x.issueDate).ToList();
                
                if (tnns.Count is 0)
                    throw new Exception("Не найдены транспортные накладные ВСД");

                var tnn = tnns[0];

                var waybill = new Waybill
                {
                    typeSpecified = true,
                    type = tnn.type,
                    issueSeries = tnn.issueSeries,
                    issueNumber = tnn.issueNumber
                };

                if (tnn.issueDateSpecified)
                {
                    waybill.issueDateSpecified = true;
                    waybill.issueDate = tnn.issueDate;
                }

                var requestData = new ProcessIncomingConsignmentRequest
                {
                    localTransactionId = localTransactionId,
                    initiator = new User {login = user.MercuryLogin},
                    delivery = new Delivery
                    {
                        accompanyingForms = new ConsignmentDocumentList
                        {
                            vetCertificate = new[]
                            {
                                new VetDocument
                                {
                                    uuid = uuid
                                }
                            },
                            waybill = waybill
                        },
                        consignment = new[]
                        {
                            consignment
                        },
                        consignor = vetDocumentItem.consignor,
                        consignee = vetDocumentItem.consignee,
                        broker = vetDocumentItem.broker,
                        transportInfo = vetDocumentItem.transportInfo,
                        transportStorageType = vetDocumentItem.transportStorageType,
                        transportStorageTypeSpecified = true,
                        deliveryDate = processDate ?? vetDocument.vetDocument.issueDate,
                        deliveryDateSpecified = true
                    },
                    deliveryFacts = new DeliveryFactList
                    {
                        vetCertificatePresence = DocumentNature.ELECTRONIC,
                        docInspection = new DeliveryInspection
                        {
                            result = DeliveryInspectionResult.CORRESPONDS
                        },
                        vetInspection = new DeliveryInspection
                        {
                            result = DeliveryInspectionResult.CORRESPONDS
                        },
                        decision = DeliveryDecision.ACCEPT_ALL
                    }
                };

                await requestData.SendRequest<ProcessIncomingConsignmentResponse>(
                    user.ApiKey,
                    _mercuryOptions.ServiceId,
                    user.IssuerId,
                    user.ApiLogin,
                    user.ApiPassword
                );
            }
            catch (Exception e)
            {
                error = e.Message;
                Console.WriteLine(e);
            }
            finally
            {
                await _mediator.Send(new VsdProcessTransactionFinish
                {
                    VsdProcessTransactionId = vsdProcessTransactionId,
                    Error = error
                });
            }
        }
    }
}
