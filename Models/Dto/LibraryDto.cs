using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models.Dto;

public class LibraryDto
{
    public string Id { get; set; } = default!;

    [DataType(DataType.DateTime)]
    public DateTime? LastSynchronized { get; set; }

    public bool IsEnabled { get; set; } = false;

    public IList<BoardGameLibraryDataDto> LibraryData { get; set; } = default!;

    public string OwnerId { get; set; } = default!;
}
