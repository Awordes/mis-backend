using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.SendModels.CommonModels
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/mercury/vet-document/v2")]
    public enum VetDocumentType
    {
        INCOMING,
        OUTGOING,
        PRODUCTIVE,
        RETURNABLE,
        TRANSPORT
    }
}
