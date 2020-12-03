using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels.Data
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    [XmlInclude(typeof(GetVetDocumentListData))]
    public abstract class IData
    {
    }
}
