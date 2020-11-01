namespace Slot.Model
{
    public interface IConvertibleToResponseXml
    {
        XmlType XmlType { get; }

        ResponseXml ToResponseXml(ResponseXmlFormat format);
    }
}