using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models.Dto;

public class BoardGameLibraryDataDto
{
    public int Id { get; set; }

    public string? Location { get; set; }

    public BoardGameDto BoardGame { get; set; } = default!;

    public string LibraryId { get; set; } = default!;
}
