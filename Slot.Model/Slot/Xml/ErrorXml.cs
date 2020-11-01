using System;
using System.Xml.Serialization;

namespace Slot.Model
{
    [Serializable]
    [XmlRoot(ElementName = "error")]
    public class ErrorXml : ResponseXml
    {
        private ErrorXml()
        {
        }

        [XmlAttribute(AttributeName = "code")]
        public ErrorCode Code { get; set; }

        [XmlText]
        public string Text { get; set; }

        public override XmlType XmlType
        {
            get { return XmlType.ErrorXml; }
        }

        public static ErrorXml Create(ErrorCode errorCode)
        {
            var errorXml = new ErrorXml
                               {
                                   Code = errorCode, 
                                   Text =
                                       Resources.ErrorCode.ResourceManager.GetString(
                                           string.Format("_{0:000}", (int)errorCode))
                               };

            return errorXml;
        }
    }
}