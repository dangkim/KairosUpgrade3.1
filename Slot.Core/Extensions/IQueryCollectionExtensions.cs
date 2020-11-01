using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;


namespace Slot.Core.Extensions
{
    public static class IQueryCollectionExtensions
    {
        public static bool TryGetDecimal(this IQueryCollection query, string key, out decimal value)
        {
            if (query.TryGetValue(key, out StringValues values) &&
                decimal.TryParse(values.FirstOrDefault(), out value))
            {
                return true;
            }
            value = 0m;
            return false;
        }

        public static bool TryGetInt32(this IQueryCollection query, string key, out int value)
        {
            if (query.TryGetValue(key, out StringValues values) &&
                int.TryParse(values.FirstOrDefault(), out value))
            {
                return true;
            }
            value = 0;
            return false;
        }

        public static bool TryGetString(this IQueryCollection query, string key, out string value)
        {
            if (query.TryGetValue(key, out StringValues values) &&
                !string.IsNullOrWhiteSpace(values.FirstOrDefault()))
            {
                value = values.First();
                return true;
            }
            value = string.Empty;
            return false;
        }
    }
}
