using System;
using System.Xml.Linq;
using System.Collections.Generic;


namespace Slot.Model.Utility
{
    public static partial class Extension
    {
        public static bool IsSubclassOfParameterlessGenericType(this Type type, Type genericType)
        {
            while (type != null && type != typeof(object))
            {
                var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericType == currentType) return true;
                type = type.BaseType;
            }

            return false;
        }

        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (firstIndex == secondIndex) return;

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static string AttributeValue(this XElement element, XName name)
        {
            if (element == null) return String.Empty;
            XAttribute attribute = element.Attribute(name);
            return attribute == null ? String.Empty : attribute.Value;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
