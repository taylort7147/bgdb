using System.Xml.Linq;

namespace BggExt.Web;

public class Downloader(HttpClient _httpClient)
{
    public async Task<Stream> Download(Uri uri)
    {
        return await _httpClient.GetStreamAsync(uri);
    }

    public async Task<string> DownloadString(Uri uri)
    {
        var reader = new StreamReader(await Download(uri));
        return reader.ReadToEnd();
    }

    public async Task<XElement> DownloadXml(Uri uri)
    {
        return await XElement.LoadAsync(await Download(uri), LoadOptions.None, CancellationToken.None);
    }

    public async Task DownloadToFile(Uri uri, string filePath)
    {
        var downloadStream = await Download(uri);
        using(var fileStream = File.OpenWrite(filePath))
        {
            await downloadStream.CopyToAsync(fileStream);
        }
    }

    public async Task<byte[]> DownloadByteArray(Uri uri)
    {
        return await _httpClient.GetByteArrayAsync(uri);
    }
}
