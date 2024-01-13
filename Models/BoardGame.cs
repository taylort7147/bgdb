using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class BoardGame
{
    public int Id { get; set; }

    [Required]
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

    public virtual IList<Mechanic>? Mechanics { get; set; } = new List<Mechanic>();

    public virtual IList<Category>? Categories { get; set; } = new List<Category>();

    public virtual IList<Family>? Families { get; set; } = new List<Family>();

    public int ThumbnailId { get; set; }
    public virtual Image? Thumbnail { get; set; }

    public int ImageId { get; set; }
    public virtual Image? Image { get; set; }
    
}
