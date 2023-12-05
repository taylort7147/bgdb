using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class BoardGame
    {
        public int Id { get; set; }

        [Required()]
        public string Name { get; set; }  = default!;

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
    }
}