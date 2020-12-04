using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Requests
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions")]
    public class SubmitApplicationRequest
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions",  Order = 0, ElementName = "apiKey")]
        public string apiKey { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 1, ElementName = "application")]
        public Common.Application application { get; set; }
    }
}
