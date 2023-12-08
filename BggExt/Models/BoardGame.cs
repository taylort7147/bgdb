using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class BoardGame
{
    public int Id { get; set; }

    [Required()]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public int YearPublished { get; set; }

    public int MinPlayers { get; set; }

    public int MaxPlayers { get; set; }

    public int PlayingTimeMinutes { get; set; }

    public int MinPlayTimeMinutes { get; set; }

    public int MaxPlayTimeMinutes { get; set; }

    public int MinAge { get; set; }

    public double AverageWeight { get; set; }

    public ICollection<Mechanic>? Mechanics { get; set; }

    public ICollection<Category>? Categories { get; set; }

    public ICollection<Family>? Families { get; set; }

    public Image? Thumbnail { get; set; }

    public Image? Image { get; set; }


    public static BoardGame ExampleBoardGame() =>
        new()
        {
            Name = "Example Board Game",
            Description = "This is a random board game",
            YearPublished = 2010,
            MinPlayers = 1,
            MaxPlayers = 4,
            PlayingTimeMinutes = 30,
            MinPlayTimeMinutes = 15,
            MaxPlayTimeMinutes = 60,
            MinAge = 10,
            AverageWeight = 2.5,
            Categories = new List<Category>
            {
                new() { Name = "Category 1" },
                new() { Name = "Category 2" },
                new() { Name = "Category 3" }
            },
            Mechanics = new List<Mechanic>
            {
                new() { Name = "Mechanic 1" },
                new() { Name = "Mechanic 2" },
                new() { Name = "Mechanic 3" }
            },
            Families = new List<Family>
            {
                new() { Name = "Family 1" },
                new() { Name = "Family 2" },
                new() { Name = "Family 3" }
            },
            Thumbnail = new Image
            {
                ImageData = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            },
            Image = new Image
            {
                ImageData = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            }
        };
}
