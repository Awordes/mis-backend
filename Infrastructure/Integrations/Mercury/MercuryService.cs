using System.Threading;
using System;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using MercuryAPI;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Core.Domain.Mercury;

namespace Infrastructure.Integrations.Mercury
{
    public class MercuryService : IMercuryService
    {
        private readonly MercuryOptions _mercuryOptions;

        public MercuryService(IOptionsMonitor<MercuryOptions> mercuryOptions)
        {
            _mercuryOptions = mercuryOptions.CurrentValue;
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

            var request = new submitApplicationRequest
                {
                    apiKey = _mercuryOptions.ApiKey,
                    application = new Application
                    {
                        serviceId = _mercuryOptions.ServiceId,
                        issuerId = _mercuryOptions.IssuerId,
                        issueDate = DateTime.Now,
                        issueDateSpecified = true,
                        data = new ApplicationDataWrapper
                        {
                            Any = requestData.Serialize()
                        }
                    }
                };

            var client = new ApplicationManagementServicePortTypeClient();

            client.ClientCredentials.UserName.UserName = _mercuryOptions.ApiLogin;
            client.ClientCredentials.UserName.Password = _mercuryOptions.ApiPassword;
            var applicationResponse = await client.submitApplicationRequestAsync(request);
            var applicationId = applicationResponse.submitApplicationResponse.application.applicationId;
            var resultRequest = new receiveApplicationResultRequest  
                {  
                    apiKey = _mercuryOptions.ApiKey,
                    applicationId = applicationId,
                    issuerId = _mercuryOptions.IssuerId
                };  

            var receiveApplicationResponse = new receiveApplicationResultResponse1();

            do
            {
                Thread.Sleep(1000);
                receiveApplicationResponse = await client.receiveApplicationResultAsync(resultRequest);
            } while(receiveApplicationResponse.receiveApplicationResultResponse.application.status == ApplicationStatus.IN_PROCESS);

            var result = receiveApplicationResponse.receiveApplicationResultResponse
                .application.result.Deserialize<GetVetDocumentListResponse>();

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
                    ExpirationDate = ComplexToDateTime(item.batch.expiryDate.firstDate),
                    ProductDate = ComplexToDateTime(item.batch.dateOfProduction.firstDate)
                }; 

                vsds.Add(element);
            }
            
            return vsds;  
        }

        private DateTime ComplexToDateTime(ComplexDate complexDate)
        {
            return new DateTime(                    
                complexDate.year,
                complexDate.month,
                complexDate.day,
                complexDate.hour,
                complexDate.minute,
                0
            );
        }
    }
}
