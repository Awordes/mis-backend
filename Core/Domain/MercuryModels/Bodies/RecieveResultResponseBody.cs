using System.Xml.Serialization;
using Core.Domain.MercuryModels.Requests;

namespace Core.Domain.MercuryModels.Bodies
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class RecieveResultResponseBody: BodyAbstract
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions",
            Order = 0, ElementName = "receiveApplicationResultResponse")]
        public ReceiveApplicationResultResponse receiveApplicationResultResponse { get; set; }
    }
}
