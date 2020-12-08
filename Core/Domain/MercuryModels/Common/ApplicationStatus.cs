using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Common
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public enum ApplicationStatus
    {
        ACCEPTED,

        IN_PROCESS,

        COMPLETED,

        REJECTED
    }
}
