using System;
using System.Xml.Serialization;

namespace Slot.Model
{
    [Serializable]
    [XmlRoot(ElementName = "INFO")]
    public class InfoXml
    {
        [XmlElement(ElementName="FUNPLAYDEMO")]
        public int FunPlayDemo { get; set; }

        [XmlElement(ElementName = "FUNPLAY")]
        public int FunPlay { get; set; }

        [XmlElement(ElementName = "EXTRAINFO")]
        public string ExtraInfo { get; set; }
    }
}
