using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions")]
    public class SubmitApplicationRequest
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions",  Order = 0, ElementName = "apiKey")]
        public string apiKey { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 1, ElementName = "application")]
        public Application application { get; set; }
    }
}
