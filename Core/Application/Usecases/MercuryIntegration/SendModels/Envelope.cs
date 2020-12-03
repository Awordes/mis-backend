using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", Order = 0, ElementName = "Header")]
        public string Header { get; set; }

        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", Order = 1, ElementName = "Body")]
        public Body Body { get; set; }
    }
}
