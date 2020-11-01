using System;
using System.Xml.Linq;
using Slot.Model.Utility;

namespace Slot.Model {
    public abstract class ResponseXml : IGameResult {
        public abstract XmlType XmlType { get; }

        protected void CreateBalanceXElement(XElement element, Balance balance) {
            string credit = 0m.ToCustomString();
            string conv = 0m.ToCustomString();
            string value = 0m.ToCustomString();

            if (balance != null) {
                credit = balance.Credit.ToCustomString();
                conv = balance.Conversion.ToCustomString();
                value = balance.Value.ToCustomString();
            }

            var childElement = new XElement("balance", value);

            element.Add(childElement);
        }

        protected XElement CreateBalanceXElement(Balance balance) {
            string credit = 0m.ToCustomString();
            string conv = 0m.ToCustomString();
            string value = 0m.ToCustomString();

            if (balance != null) {
                credit = balance.Credit.ToCustomString();
                conv = balance.Conversion.ToCustomString();
                value = balance.Value.ToCustomString();
            }
            var element = new XElement("balance", value);
            element.SetAttributeValue("credit", credit);
            element.SetAttributeValue("rate", conv);
            return element;
        }
    }
}