using System;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using MercuryAPI;
using System.Collections.Generic;
using Core.Application.Common;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Mercury;
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
        
        public async Task<object> GetVetDocumentList(
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

                var vsds = new List<Vsd>();

                foreach (var vetDocument in result.vetDocumentList.vetDocument)
                {
                    var item = (CertifiedConsignment) vetDocument.Item;

                    var element = new Vsd
                    {
                        Id = vetDocument.uuid,
                        Name = item.batch.productItem.name,
                        ProductGlobalId = item.batch.productItem.globalID,
                        Volume = item.batch.volume,
                        ExpirationDate = item.batch.expiryDate.firstDate.ToDateTime(),
                        ProductDate = item.batch.dateOfProduction.firstDate.ToDateTime()
                    }; 

                    vsds.Add(element);
                }
            
                return vsds;
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
    }
}
