using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Bodies
{
    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlInclude(typeof(SubmitRequestBody))]
    [XmlInclude(typeof(SubmitResponseBody))]
    [XmlInclude(typeof(RecieveResultRequestBody))]
    public class BodyAbstract
    {

    }
}
