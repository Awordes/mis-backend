using System;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MercuryAPI;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Microsoft.Extensions.Options;

namespace Infrastructure.Integrations.Mercury
{
    public class MercuryService : IMercuryService
    {
        private readonly MercuryOptions _mercuryOptions;

        public MercuryService(IOptionsMonitor<MercuryOptions> mercuryOptions)
        {
            _mercuryOptions = mercuryOptions.CurrentValue;
        }

        public EnumElementListViewModel GetVsdTypeListViewModel()
        {
            return VetDocumentType.INCOMING.GetDisplayNames();
        }

        public EnumElementListViewModel GetVsdStatusListViewModel()
        {
            return VetDocumentStatus.CREATED.GetDisplayNames();
        }
        
        public async Task<VsdListViewModel> GetVetDocumentList(
            string localTransactionId,
            string initiatorLogin,
            int count,
            int offset,
            int vetDocumentType,
            int vetDocumentStatus,
            string enterpriseId
            )
        {
            try
            {
                var requestData = new GetVetDocumentListRequest
                {
                    localTransactionId = localTransactionId,
                    initiator = new User { login = initiatorLogin },
                    listOptions = new ListOptions { count = count.ToString(), offset = offset.ToString() },
                    vetDocumentTypeSpecified = true,
                    vetDocumentType = (VetDocumentType)vetDocumentType,
                    vetDocumentStatusSpecified = true,
                    vetDocumentStatus = (VetDocumentStatus)vetDocumentStatus,
                    enterpriseGuid = enterpriseId
                };

                var result = await requestData.SendRequest<GetVetDocumentListResponse>(
                    _mercuryOptions.ApiKey,
                    _mercuryOptions.ServiceId,
                    _mercuryOptions.IssuerId,
                    _mercuryOptions.ApiLogin,
                    _mercuryOptions.ApiPassword
                );

                var vetDocumentList = new List<VsdViewModel>();

                foreach (var vetDocument in result.vetDocumentList.vetDocument)
                {
                    var item = (CertifiedConsignment) vetDocument.Item;

                    var element = new VsdViewModel
                    {
                        Id = vetDocument.uuid,
                        Name = item.batch.productItem.name,
                        ProductGlobalId = item.batch.productItem.globalID,
                        Volume = item.batch.volume,
                        ExpirationDate = item.batch.expiryDate.firstDate.ToDateTime(),
                        ProductDate = item.batch.dateOfProduction.firstDate.ToDateTime()
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
            string uuid,
            string enterpriseId,
            string localTransactionId,
            string initiatorLogin
            )
        {
            try
            {
                var requestData = new GetVetDocumentByUuidRequest
                {
                    localTransactionId = localTransactionId,
                    initiator = new User {login = initiatorLogin},
                    enterpriseGuid = enterpriseId,
                    uuid = uuid
                };

                var result = await requestData.SendRequest<GetVetDocumentByUuidResponse>(
                    _mercuryOptions.ApiKey,
                    _mercuryOptions.ServiceId,
                    _mercuryOptions.IssuerId,
                    _mercuryOptions.ApiLogin,
                    _mercuryOptions.ApiPassword
                );

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<object> ProcessIncomingConsignment(
            string uuid,
            string enterpriseId,
            string localTransactionId,
            string initiatorLogin)
        {
            try
            {
                var vetDocument = (GetVetDocumentByUuidResponse) await GetVetDocumentByUuid(uuid, enterpriseId, localTransactionId, initiatorLogin);

                var vetDocumentItem = (CertifiedConsignment) vetDocument.vetDocument.Item;
                
                var batch = vetDocumentItem.batch;

                var mapper = new Mapper(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Batch, Consignment>();
                }));
                
                var consignment = mapper.Map<Consignment>(batch);

                var tnn = vetDocument.vetDocument.referencedDocument.First(x => x.type == DocumentType.Item1);
                
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
                    initiator = new User { login = initiatorLogin },
                    delivery = new Delivery
                    {
                        accompanyingForms = new ConsignmentDocumentList
                        {
                            vetCertificate = new []
                            {
                                new VetDocument
                                {
                                    uuid = uuid
                                }
                            },
                            waybill = waybill
                        },
                        consignment = new []
                        {
                            consignment
                        },
                        consignor = vetDocumentItem.consignor,
                        consignee = vetDocumentItem.consignee,
                        transportInfo = vetDocumentItem.transportInfo,
                        transportStorageType = vetDocumentItem.transportStorageType,
                        transportStorageTypeSpecified = true,
                        deliveryDate = vetDocument.vetDocument.lastUpdateDate,
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
                
                var result = await requestData.SendRequest<ProcessIncomingConsignmentResponse>(
                    _mercuryOptions.ApiKey,
                    _mercuryOptions.ServiceId,
                    _mercuryOptions.IssuerId,
                    _mercuryOptions.ApiLogin,
                    _mercuryOptions.ApiPassword
                );

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
