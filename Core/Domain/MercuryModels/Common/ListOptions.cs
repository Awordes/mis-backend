using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Common
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/base")]
    public class ListOptions
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/base", Order = 0, ElementName = "count")]
        public int count { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/base", Order = 1, ElementName = "offset")]
        public int offset { get; set; }
    }
}
