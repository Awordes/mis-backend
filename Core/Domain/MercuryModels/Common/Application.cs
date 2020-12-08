using Core.Domain.MercuryModels.Data;
using Core.Domain.MercuryModels.Results;
using System;
using System.Xml.Serialization;

namespace Core.Domain.MercuryModels.Common
{
    [XmlType(Namespace = "http://api.vetrf.ru/schema/cdm/application")]
    public class Application
    {
        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 0, ElementName = "applicationId")]
        public string applicationId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 1, ElementName = "status")]
        public ApplicationStatus status { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 2, ElementName = "serviceId")]
        public string serviceId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 3, ElementName = "issuerId")]
        public string issuerId { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 4, ElementName = "issueDate")]
        public DateTime issueDate { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 5, ElementName = "rcvDate")]
        public DateTime rcvDate { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 6, ElementName = "prdcRsltDate")]
        public DateTime prdcRsltDate { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 7, ElementName = "data")]
        public DataAbstract data { get; set; }

        [XmlElement(Namespace = "http://api.vetrf.ru/schema/cdm/application", Order = 8, ElementName = "result")]
        public ResultAbstract result { get; set; }

        //BusinessError[] errors
    }
}