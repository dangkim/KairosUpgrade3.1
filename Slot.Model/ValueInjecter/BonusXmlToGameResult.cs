using System;
using System.Linq;
using System.Reflection;
using Omu.ValueInjecter.Injections;
using Omu.ValueInjecter.Utils;
using Slot.Model.Utility;


namespace Slot.Model
{
    public class BonusXmlToGameResult : KnownSourceInjection<BonusXml>
    {
        protected override void Inject(BonusXml source, object target)
        {
            var targetProps = target.GetProps();

            for (int i = 0; i < targetProps.Count(); i++)
            {
                var activeTarget = targetProps[i];
                var category = activeTarget.GetCustomAttribute<System.ComponentModel.CategoryAttribute>();
                if (category == null) continue;

                if (category.Category == "Bonus Attributes")
                {
                    var key = activeTarget.Name;

                    if (source.Attributes.TryGetValue(key, out string value))
                    {
                        activeTarget.SetValue(target, Convert.ChangeType(value, activeTarget.PropertyType));
                    }
                }
                else if (category.Category == "Data")
                {
                    var type = Type.GetType(activeTarget.Name);

                    var method = typeof(XmlHelper).GetMethod("Deserialize");
                    var generic = method.MakeGenericMethod(type);

                    var serializer = new XmlHelper();
                    var responseXml = generic.Invoke(serializer, new object[] { source.Data.FirstNode.ToString() });

                    if (!(responseXml is IConvertibleToGameResult convertibleXml))
                    {
                        continue;
                    }

                    var gameResult = convertibleXml.ToGameResult();

                    activeTarget.SetValue(target, Convert.ChangeType(gameResult, activeTarget.PropertyType));
                }
            }
        }
    }
}