using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class BoardGameLibraryData
{
    public int Id { get; set; }

    public string? Location { get; set; }

    public BoardGame BoardGame { get; set; } = default!;

    public Library Library { get; set; } = default!;
}
