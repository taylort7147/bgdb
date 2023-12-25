using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class Library
{
    [Key]
    public string Id { get; set; } = default!;

    [DataType(DataType.DateTime)]
    public DateTime? LastSynchronized { get; set; }

    public bool IsSynchronizationEnabled { get; set; } = false;

    public virtual IList<BoardGameLibraryData> LibraryData { get; set; } = default!;

    public virtual ApplicationUser Owner { get; set; } = default!;
}
