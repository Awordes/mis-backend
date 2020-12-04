using Core.Domain.MercuryModels.Methods;
using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Data
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public class GetVetDocumentListData: DataAbstract
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2", 
            Order = 0, ElementName = "getVetDocumentListRequest")]
        public GetVetDocumentListRequest getVetDocumentListRequest { get; set; }
    }
}
