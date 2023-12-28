using System.Xml.Linq;

namespace BggExt.XmlApi2;

public class Link
{
    public int Id { get; set; }

    public string Type { get; set; } = default!;

    public string Value { get; set; } = default!;

    public Link()
    {
    }

    public Link(XElement xml)
    {
        Id = xml.GetAttributeValue<int>("id");
        Type = xml.GetAttributeValue("type");
        Value = xml.GetAttributeValue("value");
    }
}
