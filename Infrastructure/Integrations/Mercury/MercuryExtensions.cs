using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using MercuryAPI;

namespace Infrastructure.Integrations.Mercury
{
    public static class MercuryExtensions
    {
        public static XmlElement Serialize(this MercuryApplicationRequest requestData)
        {
            var result = new XmlDocument();

            var requestType = requestData.GetType();

            var rootElement = new XmlRootAttribute
            {
                ElementName = requestType.ToCamelCase(),
                Namespace = requestType.GetCustomAttribute<XmlTypeAttribute>().Namespace
            };

            using (var writer = result.CreateNavigator().AppendChild())
            {
                new XmlSerializer(requestType, rootElement)
                    .Serialize(writer, requestData);
            }

            return result.DocumentElement;
        }
        public static TResponse Deserialize<TResponse>(this ApplicationResultWrapper wrapper)  
            where TResponse : ApplicationResultData  
        {  
            var responseType = typeof(TResponse);  
            var rootAttribute = new XmlRootAttribute  
            {  
                ElementName = responseType.ToCamelCase(),  
                Namespace = responseType.GetCustomAttribute<XmlTypeAttribute>().Namespace  
            };  
            var serializer = new XmlSerializer(responseType, rootAttribute);  
            return (TResponse) serializer.Deserialize(new XmlNodeReader(wrapper.Any));  
        } 

        private static string ToCamelCase(this Type requestType)  
        {  
            return $"{char.ToLower(requestType.Name[0])}{requestType.Name.Substring(1)}";  
        }
    }
}
