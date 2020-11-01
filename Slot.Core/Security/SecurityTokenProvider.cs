using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Slot.Core.Security
{
    public static class SecurityTokenProvider
    {
        public static string Create(string token)
        {
            var algorithm = CryptographyHelpers.CreateSHA256();
            var bytes = Encoding.ASCII.GetBytes($"{token}-{DateTime.Now.Ticks}");
            var hashed = algorithm.ComputeHash(bytes);
            var result = Base64UrlEncoder.Encode(hashed);
            return result;
        }

        public static string Create()
        {
            return Create(Guid.NewGuid().ToString());
        }
    }
}
