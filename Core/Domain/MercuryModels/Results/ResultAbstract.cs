using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Results
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    [XmlInclude(typeof(GetVetDocumentListResult))]
    public class ResultAbstract
    {
    }
}
