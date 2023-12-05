
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BggExt.XmlApi2
{
    public class BoardGame
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public int YearPublished { get; set; }

        public int MinPlayers { get; set; } = 0;

        public int MaxPlayers { get; set; } = int.MaxValue;

        public int PlayingTimeMinutes { get; set; } = 0;

        public int MinPlayTimeMinutes { get; set; } = 0;

        public int MaxPlayTimeMinutes { get; set; } = int.MaxValue;

        public int MinAge { get; set; } = 0;

        public double AverageWeight { get; set; } = int.MaxValue;

        public IList<Link>? Mechanics { get; set; }

        public IList<Link>? Categories { get; set; }

        public IList<Link>? Families { get; set; }

        public Uri? Thumbnail { get; set; }

        public Uri? Image { get; set; }

        public BoardGame()
        {
        }

        public BoardGame(XElement xml)
        {
            Id = xml.GetAttributeValue<int>("id");
            Name = xml.Elements("name")
                .Where(e => e.GetAttributeValueOrDefault("type", "") == "primary")
                .First().GetAttributeValue("value");
            Description = xml.Element("description").GetElementValueOrDefault("");
            YearPublished = xml.Element("yearpublished").GetAttributeValueOrDefault("value", 0);
            MinPlayers = xml.Element("minplayers").GetAttributeValueOrDefault("value", 0);
            MaxPlayers = xml.Element("maxplayers").GetAttributeValueOrDefault("value", int.MaxValue);
            PlayingTimeMinutes = xml.Element("playingtime").GetAttributeValueOrDefault("value", 0);
            MinPlayTimeMinutes = xml.Element("minplaytime").GetAttributeValueOrDefault("value", 0);
            MaxPlayTimeMinutes = xml.Element("maxplaytime").GetAttributeValueOrDefault("value", int.MaxValue);
            MinAge = xml.Element("minage").GetAttributeValueOrDefault("value", 0);
            AverageWeight = xml.XPathSelectElement("statistics/ratings/averageweight").GetAttributeValueOrDefault("value", 0.0);
            AverageWeight = xml.Descendants("averageweight").First().GetAttributeValueOrDefault("value", 0.0);
            Thumbnail = new Uri(xml.Element("thumbnail").GetElementValue());
            Image = new Uri(xml.Element("image").GetElementValue());

            Mechanics = new List<Link>();
            Categories = new List<Link>();
            Families = new List<Link>();
            foreach (XElement element in xml.Descendants("link"))
            {
                var link = new Link(element);
                if (link.Type == "boardgamemechanic")
                {
                    Mechanics.Add(link);
                }
                else if (link.Type == "boardgamecategory")
                {
                    Categories.Add(link);
                }
                else if (link.Type == "boardgamefamily")
                {
                    Families.Add(link);
                }
            }
        }
    }
}