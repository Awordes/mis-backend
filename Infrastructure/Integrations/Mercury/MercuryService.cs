using System;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MercuryAPI;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Integrations.Mercury
{
    public class MercuryService : IMercuryService
    {
        private readonly MercuryOptions _mercuryOptions;
        private readonly ILogService _logService;
        private readonly ILogger<MercuryService> _logger;

        public MercuryService(
            IOptionsMonitor<MercuryOptions> mercuryOptions,
            ILogService logService,
            ILogger<MercuryService> logger)
        {
            _mercuryOptions = mercuryOptions.CurrentValue;
            _logService = logService;
            _logger = logger;
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

                if (result.vetDocumentList.vetDocument is not null)
                    foreach (var vetDocument in result.vetDocumentList.vetDocument)
                    {
                        var item = (CertifiedConsignment) vetDocument.Item;

                        DateTime? processDate = null;

                        if (vetDocument.referencedDocument is not null)
                        {
                            var tnns = vetDocument.referencedDocument
                                .Where(x => x.type == DocumentType.Item1 || x.type == DocumentType.Item5)
                                .OrderByDescending(x => x.issueDate)
                                .ToList();

                            processDate = tnns[0]?.issueDate.AddDays(1);
                        }
                        
                        var element = new VsdViewModel
                        {
                            Id = vetDocument.uuid,
                            Name = item.batch.productItem.name,
                            ProductGlobalId = item.batch.productItem.globalID,
                            Volume = item.batch.volume,
                            ProductDate = item.batch.dateOfProduction.firstDate.ToDateTime(),
                            IssueDate = vetDocument.issueDate,
                            ProcessDate = processDate
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
                _logger.LogError(e, e.Message);
                
                if (e is EndpointNotFoundException)
                    return new VsdListViewModel { result = new List<VsdViewModel>(), ElementCount = 0 };

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
                _logger.LogError(e, e.Message);
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
            try
            {
                var vsdProcessTransactionId = await _logService.StartVsdProcessTransaction(operationId, uuid);
                
                string error = null;

                try
                {
                    var vetDocument = (GetVetDocumentByUuidResponse) await GetVetDocumentByUuid(
                        localTransactionId, user, enterprise, uuid);

                    var vetDocumentItem = (CertifiedConsignment) vetDocument.vetDocument.Item;

                    var batch = vetDocumentItem.batch;

                    var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<Batch, Consignment>(); }));

                    var consignment = mapper.Map<Consignment>(batch);

                    List<ReferencedDocument> tnns = null;

                    if (vetDocument.vetDocument.referencedDocument is not null)
                    {
                        tnns = vetDocument.vetDocument.referencedDocument
                            .Where(x => x.type is DocumentType.Item1 or DocumentType.Item5)
                            .OrderByDescending(x => x.issueDate).ToList();
                    }
                    
                    if (tnns is null || tnns.Count == 0)
                        throw new Exception($"Не найдены транспортные накладные для ВСД {uuid}");

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

                    var isTransShipSpecified = false;
                    var transShipInfo = new TransportInfo();

                    var shipmentRoute = vetDocumentItem.shipmentRoute?.OrderByDescending(x => x.sqnId).FirstOrDefault();
                    if (shipmentRoute is not null)
                    {
                        isTransShipSpecified = shipmentRoute.transshipmentSpecified;
                        if (isTransShipSpecified)
                            transShipInfo = shipmentRoute.nextTransport;
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
                            transportInfo = isTransShipSpecified
                                            ? transShipInfo
                                            : vetDocumentItem.transportInfo,
                            transportStorageType = vetDocumentItem.transportStorageType,
                            transportStorageTypeSpecified = vetDocumentItem.transportStorageTypeSpecified,
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
                    _logger.LogError(e, e.Message);
                }
                finally
                {
                    await _logService.FinishVsdProcessTransaction(vsdProcessTransactionId, error);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
