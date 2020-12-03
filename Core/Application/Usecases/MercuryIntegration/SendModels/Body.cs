using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions", Order = 0, ElementName = "submitApplicationRequest")]
        public SubmitApplicationRequest submitApplicationRequest { get; set; }
    }
}
