using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Requests
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application/ws-definitions")]
    public class ReceiveApplicationResultResponse
    {

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 0, ElementName = "application")]
        public Common.Application application { get; set; }
    }
}
