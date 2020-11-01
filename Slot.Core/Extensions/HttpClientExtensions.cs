using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slot.Core.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<XDocument> GetXmlAsync(this HttpClient httpClient, string requestUri)
        {
            using (var stream = await httpClient.GetStreamAsync(requestUri))
            {
                return XDocument.Load(stream);
            }
        }
    }
}
