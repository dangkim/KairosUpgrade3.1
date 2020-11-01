using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Slot.Model.Utility;


namespace Slot.Model
{
    [XmlRoot(ElementName = "reelsInfo")]
    public class ReelsInfoXml : ResponseXml, IXmlSerializable
    {
        public int Height;
        public int Width;

        public Dictionary<int, List<int>> MainReels;

        public Dictionary<int, List<int>> FeatureReels;

        public override XmlType XmlType
        {
            get { return XmlType.ReelsInfoXml; }
        }

        public ReelsInfoXml()
        {
            this.MainReels = new Dictionary<int, List<int>>();
            this.FeatureReels = new Dictionary<int, List<int>>();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public XElement ToXElement()
        {
            var element = new XElement("reelsInfo");

            XElement main = new XElement("main");
            CreateReelsXML(main, MainReels);
            element.Add(main);

            XElement feature = new XElement("feature");
            CreateReelsXML(feature, FeatureReels);
            element.Add(feature);

            return element;
        }

        public void WriteXml(XmlWriter writer)
        {
            var xElement = this.ToXElement();
            foreach (var element in xElement.Elements())
            {
                element.WriteTo(writer);
            }
        }

        private void CreateReelsXML(XElement element, Dictionary<int, List<int>> reels)
        {
            for (int i = 0; i < this.Width; i++)
            {
                XElement elem = new XElement("item");
                elem.SetAttributeValue("id", i + 1);
                elem.SetAttributeValue("val", reels[i].ToCommaDelimitedString());
                element.Add(elem);
            }
        }
    }
}