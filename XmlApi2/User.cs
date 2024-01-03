using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.IdentityModel.Tokens;

namespace BggExt.XmlApi2;

public class User
{

    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? AvatarLink { get; set; }

    public int YearRegistered { get; set; }

    [DataType(DataType.Date)]
    public DateTime LastLogin { get; set; }

    public string? StateOrProvince { get; set; }

    public string? Country { get; set; }

    public string? WebAddress { get; set; }

    public string? XboxAccount { get; set; }

    public string? WiiAccount { get; set; }

    public string? PsnAccount { get; set; }

    public string? BattlnetAccount { get; set; }

    public string? SteamAccount { get; set; }

    public int TradeRating { get; set; }

    public User(XElement xml)
    {
        // TODO: Redesign the getters to allow nullables
        Id = xml.GetAttributeValue<int>("id");
        Name = xml.GetAttributeValue<string>("name");
        FirstName = xml.Element("firstname").GetAttributeValue("value");
        LastName = xml.Element("lastname").GetAttributeValue("value");
        AvatarLink = xml.Element("avatarlink").GetAttributeValue("value");
        YearRegistered = xml.Element("yearregistered").GetAttributeValue<int>("value");
        LastLogin = DateTime.Parse(xml.Element("lastlogin").GetAttributeValue("value"));
        StateOrProvince = xml.Element("stateorprovince").GetAttributeValue("value");
        Country = xml.Element("country").GetAttributeValue("value");
        WebAddress = xml.Element("webaddress").GetAttributeValue("value");
        XboxAccount = xml.Element("xboxaccount").GetAttributeValue("value");
        WiiAccount = xml.Element("wiiaccount").GetAttributeValue("value");
        PsnAccount = xml.Element("psnaccount").GetAttributeValue("value");
        BattlnetAccount = xml.Element("battlenetaccount").GetAttributeValue("value");
        SteamAccount = xml.Element("steamaccount").GetAttributeValue("value");
        TradeRating = xml.Element("traderating").GetAttributeValue<int>("value");
    }
}
