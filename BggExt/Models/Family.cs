using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class Family
{
    public int Id { get; set; }

    [Required()]
    public string Name { get; set; } = default!;

    public ICollection<BoardGame> BoardGames { get; set; } = default!;
}
