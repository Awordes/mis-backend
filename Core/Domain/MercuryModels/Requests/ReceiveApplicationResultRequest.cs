using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Requests
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions")]
    public class ReceiveApplicationResultRequest
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions", Order = 0, ElementName = "apiKey")]
        public string apiKey { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions", Order = 1, ElementName = "issuerId")]
        public string issuerId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions", Order = 2, ElementName = "applicationId")]
        public string applicationId { get; set; }
    }
}
