using Slot.Model;
using System;

namespace Slot.BackOffice.Models.Xml
{
    public static class XmlTypeExtension
    {
        public static Type ToType(this XmlType xmlType)
        {
            switch (xmlType)
            {
                case XmlType.SpinXml: return typeof(SpinXml);
                case XmlType.BonusXml: return typeof(BonusXml);
            }

            return default(Type);
        }
    }
}