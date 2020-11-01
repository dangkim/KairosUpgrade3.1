using System.Collections.Generic;
using System.IO;
using Slot.Model;

namespace Slot.Core.Modules.Infrastructure
{
    public static class NonWinCombFactory
    {
        public static IReadOnlyDictionary<int, IReadOnlyList<int[]>> Create(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var str = sr.ReadToEnd();
                return Newtonsoft.Json.Linq.JObject.Parse(str).ToObject<IReadOnlyDictionary<int, IReadOnlyList<int[]>>>();
            }
        }
    }
}
