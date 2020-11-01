using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Slot.Model.Utility
{
    /// <summary>
    /// For ignore [JsonIgnore] Attribute on Serialization / Deserialization
    /// </summary>
    public class JsonIgnoreAttributeIgnorerContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Ignored = false;
            return property;
        }
    }

    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings ignoreIgnoreSettings =
            new JsonSerializerSettings { ContractResolver = new JsonIgnoreAttributeIgnorerContractResolver() };

        public static string FullString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, ignoreIgnoreSettings);
        }

        public static string ToString<T>(T obj, Newtonsoft.Json.Formatting formatting)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public static string ToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        public static T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public class XmlHelper
    {
        public T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public string Serialize(object obj)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, obj, emptyNamespaces);
                    return stream.ToString();
                }
            }
        }

        public static bool Validate<T>(string xml, string xsd)
        {
            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            var assembly = Assembly.GetAssembly(typeof(T));

            using (var schemaStream = assembly.GetManifestResourceStream(xsd))
            {
                if (schemaStream == null)
                    throw new ArgumentException(String.Format("Cannot find the manifest resource {0} from any of the manifest resource name {1}", xsd, String.Join(", ", assembly.GetManifestResourceNames())));

                XmlSchema schema = XmlSchema.Read(schemaStream, null);
                settings.Schemas.Add(schema);
            }

            XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            return true;
        }
        public static void WriteElement(XmlWriter writer, string element, object value)
        {
            writer.WriteStartElement(element);
            writer.WriteRaw(string.Format("{0}", value));
            writer.WriteEndElement();
        }
    }
}
