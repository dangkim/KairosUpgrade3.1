using System;
using System.Xml.Serialization;


namespace Slot.Model
{
    [Serializable]
    [XmlRoot(ElementName = "report")]
    public class ReportXml : ResponseXml
    {
        [XmlElement(ElementName = "bet", Order = 2)]
        public decimal Bet { get; set; }

        [XmlElement(ElementName = "currency", Order = 1)]
        public string Currency { get; set; }

        [XmlElement(ElementName = "netwin", Order = 3)]
        public decimal NetWin { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        public override XmlType XmlType
        {
            get { return XmlType.ReportXml; }
        }
    }
}