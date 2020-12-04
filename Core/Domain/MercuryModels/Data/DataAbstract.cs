using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Data
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    [XmlInclude(typeof(GetVetDocumentListData))]
    public class DataAbstract
    {
    }
}
