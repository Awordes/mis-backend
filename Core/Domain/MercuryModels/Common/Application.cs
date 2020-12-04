using Core.Domain.MercuryModels.Data;
using System;
using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Common
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public class Application
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 0, ElementName = "serviceId")]
        public string serviceId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 1, ElementName = "issuerId")]
        public string issuerId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 2, ElementName = "issueDate")]
        public DateTime issueDate { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 3, ElementName = "data")]
        public DataAbstract data { get; set; }
    }
}
