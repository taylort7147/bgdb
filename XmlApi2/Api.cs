using BggExt.Web;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;

namespace BggExt.XmlApi2;

public class Api(Downloader _downloader)
{
    private static string _ApiHost = "http://boardgamegeek.com/";

    private async Task<XElement?> _CallApi(string path)
    {
        var uri = new Uri($"{_ApiHost}/{path}");
        return await _downloader.DownloadXml(uri);
    }

    private ApiResult _ProcessApiCall(XElement? xml, string? rootElement = null)
    {
        var result = new ApiResult();
        result.Xml = xml;
        if (xml == null)
        {
            result.Errors.Add("The XML contains no root element.");
        }
        else if (string.IsNullOrEmpty(xml.Name.LocalName))
        {
            result.Errors.Add("The XML root element name is empty.");
        }
        else if (xml.Name == "errors")
        {
            result.Errors.Add("The API returned errors.");
            foreach (var element in xml.Elements()) result.Errors.Add(element.Value);
        }
        else if (xml.Name == "message")
        {
            result.Message = xml.Value;
        }
        else if (rootElement != null && xml.Name != rootElement)
        {
            result.Errors.Add($"Unexpected XML root element: <${xml.Name}>");
        }

        return result;
    }


    public async Task<ApiResult> GetBoardGame(int id)
    {
        var result = await GetBoardGames(new List<int> { id });

        // If there was an error, return immediately
        if (result.Status == ApiResult.OperationStatus.Error) return result;

        // Extract the board game from the list of board games
        var boardGames = result.Data as IList<BoardGame>;
        if (boardGames == null)
        {
            result.Data = null;
        }
        else if (boardGames.Count == 1)
        {
            result.Data = boardGames[0];
        }
        else
        {
            result.Data = null;
            result.Errors.Add("Expected exactly 1 board game but got " + boardGames.Count);
        }

        return result;
    }

    public async Task<ApiResult> GetBoardGames(IList<int> ids)
    {
        var idsString = string.Join(",", ids);
        var result = _ProcessApiCall(await _CallApi($"/xmlapi2/thing?id={idsString}&stats=1&type=boardgame"), "items");
        if (result.Status == ApiResult.OperationStatus.Success)
        {
            var root = result.Xml!;
            var boardGames = new List<BoardGame>();
            var items = root.Elements("item");
            foreach (var item in items)
            {
                ArgumentNullException.ThrowIfNull(item);
                var boardGame = new BoardGame(item);
                boardGames.Add(boardGame);
            }

            result.Data = boardGames;
        }

        return result;
    }

    public async Task<ApiResult> GetCollection(string userId)
    {
        var result = _ProcessApiCall(await _CallApi($"/xmlapi2/collection?username={userId}&subtype=boardgame&brief=1&own=1"),
            "items");
        if (result.Status == ApiResult.OperationStatus.Success)
        {
            var items = result.Xml!.Elements("item");
            var ids = items.Select(i => i.GetAttributeValue<int>("objectid")).Distinct().ToList();
            return await GetBoardGames(ids);
        }

        return result;
    }

    // TODO: Change ApiResult to a generic with data type as the template parameter
    public async Task<ApiResult> GetUser(string userId)
    {
        var result = _ProcessApiCall(await _CallApi($"/xmlapi2/user?name={userId}"), "user");
        if (result.Status == ApiResult.OperationStatus.Success)
        {
            if (result.Xml!.GetAttributeValue("id").IsNullOrEmpty())
            {
                result.Errors.Add($"The user '{userId} was not found");
            }
            else
            {
                var user = new User(result.Xml!);
                result.Data = user;
            }
        }
        return result;
    }
}
