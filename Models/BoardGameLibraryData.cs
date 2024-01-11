using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class BoardGameLibraryData
{
    public int Id { get; set; }

    public string? Location { get; set; }

    public virtual int BoardGameId { get; set; }

    public virtual string LibraryId { get; set; } = default!;

    public virtual BoardGame BoardGame { get; set; } = default!;

    public virtual Library Library { get; set; } = default!;
}
