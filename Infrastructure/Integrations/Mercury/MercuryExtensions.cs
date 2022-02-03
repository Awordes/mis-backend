using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using MercuryAPI;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using Infrastructure.Exceptions;

namespace Infrastructure.Integrations.Mercury
{
    public static class MercuryExtensions
    {
        private static XmlElement Serialize(this MercuryApplicationRequest requestData)
        {
            var result = new XmlDocument();

            var requestType = requestData.GetType();

            var rootElement = new XmlRootAttribute
            {
                ElementName = requestType.ToCamelCase(),
                Namespace = requestType.GetCustomAttribute<XmlTypeAttribute>()?.Namespace
            };

            using (var writer = result.CreateNavigator()?.AppendChild())
            {
                if (writer is not null) new XmlSerializer(requestType, rootElement).Serialize(writer, requestData);
            }

            return result.DocumentElement;
        }

        private static TResponse Deserialize<TResponse>(this ApplicationResultWrapper wrapper)  
            where TResponse : ApplicationResultData  
        {  
            var responseType = typeof(TResponse);  
            var rootAttribute = new XmlRootAttribute  
            {  
                ElementName = responseType.ToCamelCase(),  
                Namespace = responseType.GetCustomAttribute<XmlTypeAttribute>()?.Namespace  
            };  
            var serializer = new XmlSerializer(responseType, rootAttribute);  
            return (TResponse) serializer.Deserialize(new XmlNodeReader(wrapper.Any));  
        } 

        private static string ToCamelCase(this MemberInfo requestType)  
        {  
            return $"{char.ToLower(requestType.Name[0])}{requestType.Name[1..]}";  
        }
        
        public static async Task<TResponse> SendRequest<TResponse>(
            this MercuryApplicationRequest requestData,
            string apiKey,
            string serviceId,
            string issuerId,
            string apiLogin,
            string apiPassword
            ) where TResponse : ApplicationResultData
        {
            var request = new submitApplicationRequestRequest
            {
                submitApplicationRequest = new submitApplicationRequest
                {
                    apiKey = apiKey,
                    application = new Application
                    {
                        serviceId = serviceId,
                        issuerId = issuerId,
                        issueDate = DateTime.Now,
                        issueDateSpecified = true,
                        data = new ApplicationDataWrapper
                        {
                            Any = requestData.Serialize()
                        }
                    }
                }
            };

            var client = new ApplicationManagementServicePortTypeClient();
            client.ClientCredentials.UserName.UserName = apiLogin;
            client.ClientCredentials.UserName.Password = apiPassword;

            var retries = 0;
            var needToRetry = false;

            var applicationResponse = new submitApplicationRequestResponse();

            do
            {
                try
                {
                    applicationResponse = await client.submitApplicationRequestAsync(request);
                    needToRetry = false;
                }
                catch (CommunicationException e)
                {
                    if (retries > 10)
                    {
                        Console.WriteLine("MIS: Too many connection retries");
                        throw;
                    }

                    Console.WriteLine(e);
                    Console.WriteLine($"retries: {retries}");
                    retries++;
                    await Task.Delay(1000);
                    needToRetry = true;
                }
            } while (needToRetry);

            var resultRequest = new receiveApplicationResultRequest1
            {
                receiveApplicationResultRequest = new receiveApplicationResultRequest
                {
                    apiKey = apiKey,
                    applicationId = applicationResponse.submitApplicationResponse.application.applicationId,
                    issuerId = issuerId
                }
            };

            receiveApplicationResultResponse1 receiveApplicationResponse;
            ApplicationStatus status;

            do
            {
                await Task.Delay(1000);
                receiveApplicationResponse = await client.receiveApplicationResultAsync(resultRequest);
                status = receiveApplicationResponse.receiveApplicationResultResponse.application.status;
            } while (status == ApplicationStatus.IN_PROCESS);

            switch (status)
            {
                case ApplicationStatus.COMPLETED:
                    return receiveApplicationResponse.receiveApplicationResultResponse.application.result
                        .Deserialize<TResponse>();

                case ApplicationStatus.REJECTED:
                {
                    var application = receiveApplicationResponse.receiveApplicationResultResponse.application;

                    var errorMessage = "applicationId: '" + application.applicationId;

                    errorMessage = application.errors.Aggregate(errorMessage,
                        (current, error) =>
                            current + ("', errorCode: '" + error.code + "', errorMessage: '" + error.Value));

                    //Предприятие с указанным идентификатором не найдено
                    if (application.errors.Select(x => x.code).Contains("MERC31180"))
                        throw new MercuryEnterpriseNotFoundException(errorMessage);

                    throw new MercuryServiceException(errorMessage + "'");
                }

                default:
                    return null;
            }
        }
        
        public static DateTime? ToDateTime(this ComplexDate complexDate)
        {
            if (complexDate == null) return null;
            
            return complexDate.yearSpecified ? new DateTime(
                complexDate.year,
                complexDate.monthSpecified ? complexDate.month : 1,
                complexDate.daySpecified ? complexDate.day : 1,
                complexDate.hourSpecified ? complexDate.hour : 0,
                complexDate.minuteSpecified ? complexDate.minute : 0,
                0
            ) : null;
        }

        public static EnumElementListViewModel GetDisplayNames(this Enum enumElement)
        {
            var enumNames = Enum.GetNames(enumElement.GetType());

            return new EnumElementListViewModel
            {
                EnumElements = enumNames.Select((t, i) => new EnumElementViewModel
                    {
                        Id = i,
                        Name = t,
                        Title = enumElement.GetType().GetMember(t)[0]
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? ""
                    }).ToList()
            };
        }
    }
}
