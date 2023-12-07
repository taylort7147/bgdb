using BggExt.Web;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BggExt.XmlApi2;

public class Api(Downloader _downloader)
{
    private static string _ApiHost = "http://boardgamegeek.com/";

    private async Task<XElement?> _CallApi(string path)
    {
        var uri = $"{_ApiHost}/{path}";
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

    /*
    def get_collection(user_id):
        raw_data = _call_api(f"/xmlapi2/collection?username={user_id}&subtype=boardgame&brief=1")
        root = ElementTree.fromstring(raw_data)
        result = ApiResult()
        if root.tag == "errors":
            result.status = ApiResult.error
            result.errors = []
            for error in root:
                result.errors.append(error[0].text)
        elif root.tag == "message":
            result.status = ApiResult.pending
            result.message = root.text
        elif root.tag == "items":
            items = [CollectionItem.from_xml(x) for x in root]
            ids = [x.id for x in items]
            result = get_games(ids)
        return result
    */
    public async Task<ApiResult> GetCollection(string userId)
    {
        var result = _ProcessApiCall(await _CallApi($"/xmlapi2/collection?username={userId}&subtype=boardgame&brief=1"),
            "items");
        if (result.Status == ApiResult.OperationStatus.Success)
        {
            var items = result.Xml!.Elements("item");
            var ids = new List<int>(
                from item in items
                select item.GetAttributeValue<int>("objectid")
            );
            return await GetBoardGames(ids);
        }

        return result;
    }
}