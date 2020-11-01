using System.Security.Cryptography;
using System.Text;

namespace Slot.Model.Utilities
{
    public static class HashUtil
    {
        /// <summary>
        /// Compute hash for string encoded as UTF8
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <returns>40-character hex string</returns>
        public static string SHA1Hash(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
 
            return HexStringFromBytes(hashBytes);
        }

        public static string MD5Hash(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }


        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}