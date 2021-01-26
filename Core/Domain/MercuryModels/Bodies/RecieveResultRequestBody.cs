using Core.Domain.MercuryModels.Requests;
using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Bodies
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class RecieveResultRequestBody: BodyAbstract
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions",
            Order = 0, ElementName = "receiveApplicationResultRequest")]
        public ReceiveApplicationResultRequest receiveApplicationResultRequest { get; set; }
    }
}
