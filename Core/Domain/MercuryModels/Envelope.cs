using Core.Domain.MercuryModels.Bodies;
using System.Xml.Serialization;

namespace Core.Domain.MercuryModels
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", Order = 0, ElementName = "Header")]
        public string Header { get; set; }

        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", Order = 1, ElementName = "Body")]
        public BodyAbstract Body { get; set; }
    }
}
