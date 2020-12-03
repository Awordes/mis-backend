using Core.Application.Usecases.MercuryIntegration.SendModels.Requests;
using System;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels.Data
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public class GetVetDocumentListData: IData
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2", Order = 0, ElementName = "getVetDocumentListRequest")]
        public GetVetDocumentListRequest getVetDocumentListRequest { get; set; }
    }
}
