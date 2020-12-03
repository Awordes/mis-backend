using Core.Application.Usecases.MercuryIntegration.SendModels.CommonModels;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels.Requests
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vu/applications/v2")]
    public class GetVetDocumentListRequest
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2", Order = 0, ElementName = "localTransactionId")]
        public string localTransactionId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/g2b/applications/v2", Order = 1, ElementName = "initiator")]
        public Initiator initiator { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/base", Order = 2, ElementName = "listOptions")]
        public ListOptions listOptions { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2", Order = 3, ElementName = "vetDocumentType")]
        public VetDocumentType vetDocumentType { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2", Order = 4, ElementName = "vetDocumentStatus")]
        public VetDocumentStatus vetDocumentStatus { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/dictionary/v2", Order = 5, ElementName = "enterpriseGuid")]
        public string enterpriseGuid { get; set; }
    }
}