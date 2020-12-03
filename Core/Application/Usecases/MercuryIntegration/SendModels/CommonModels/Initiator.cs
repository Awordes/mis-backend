using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels.CommonModels
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vu/applications/v2")]
    public class Initiator
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2", Order = 0, ElementName = "login")]
        public string login { get; set; }
    }
}
