using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class Category
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required()]
    public string Name { get; set; } = default!;

    public virtual IList<BoardGame> BoardGames { get; set; } = default!;
}
