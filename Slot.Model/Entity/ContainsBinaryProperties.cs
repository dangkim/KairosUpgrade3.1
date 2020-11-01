using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Slot.Model.Entity
{
    public abstract class ContainsBinaryProperties
    {
        public object ByteArrayToObject(byte[] byteArray)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();

            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            object obj = binForm.Deserialize(memStream);
            return obj;
        }

        public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms, obj);

            return ms.ToArray();
        }
    }
}