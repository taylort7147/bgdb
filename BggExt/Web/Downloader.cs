using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BggExt.Web;

public class Downloader(HttpClient _httpClient)
{
    public async Task<Stream> Download(string uri)
    {
        return await _httpClient.GetStreamAsync(uri);
    }

    public async Task<string> DownloadString(string uri)
    {
        var reader = new StreamReader(await Download(uri));
        return reader.ReadToEnd();
    }

    public async Task<XElement> DownloadXml(string uri)
    {
        return await XElement.LoadAsync(await Download(uri), LoadOptions.None, CancellationToken.None);
    }
}