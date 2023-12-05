using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BggExt.Web
{
    class Downloader
    {
        public static async Task<Stream> Download(string uri)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStreamAsync(uri);
            }
        }
        public static async Task<string> DownloadString(string uri)
        {
            StreamReader reader = new StreamReader(await Download(uri));
            return reader.ReadToEnd();
        }

        public static async Task<XElement> DownloadXml(string uri)
        {
            return await XElement.LoadAsync(await Download(uri), LoadOptions.None, CancellationToken.None);
        }
    }
}