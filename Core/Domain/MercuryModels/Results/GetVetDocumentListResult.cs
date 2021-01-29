using System.Xml.Serialization;
using Core.Domain.MercuryModels.Methods;

namespace Core.Domain.MercuryModels.Results
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public class GetVetDocumentListResult: ResultAbstract
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2", 
            Order = 0, ElementName = "getVetDocumentListResponse")]
        public GetVetDocumentListResponse getVetDocumentListResponse { get; set; }
    }
}
