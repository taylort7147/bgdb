using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models.Dto;

public class BoardGameDto
{
    public int Id { get; set; }

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

    public IList<string>? Mechanics { get; set; }

    public IList<string>? Categories { get; set; }

    public IList<string>? Families { get; set; }

    public int ThumbnailId { get; set; }

    public int ImageId { get; set; }
    
}
