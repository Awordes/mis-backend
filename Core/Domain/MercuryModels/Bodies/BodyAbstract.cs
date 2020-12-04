using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Bodies
{
    [XmlInclude(typeof(SubmitRequestBody))]
    [XmlInclude(typeof(RecieveResponseBody))]
    public abstract class BodyAbstract
    {

    }
}
