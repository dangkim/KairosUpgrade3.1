using System;
using System.Net;
using System.Xml.Linq;


namespace Slot.Model.Utility
{
    public class HttpUtil
    {
        public static string GetResponse(string url, string prm)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return wc.DownloadString(String.Format("{0}?{1}", url, prm));
            }
        }

        public static XDocument GetResponseXml(string url, string prm)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string adr = String.Format("{0}?{1}", url, prm);
                return XDocument.Parse(wc.DownloadString(adr));
            }
        }

        public static string GetResponsePost(string url, string prm)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Accept] = "application/json;charset=utf-8,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return wc.UploadString(url, "POST", prm);
            }
        }
    }
}
