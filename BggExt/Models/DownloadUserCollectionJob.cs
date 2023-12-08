using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class DownloadLibraryJob
{
    [Required()] 
    public string UserId { get; set; } = default!;

    public bool IsActive { get; set; }

    public bool WasSuccessful { get; set; }

    public string? Details { get; set; }
}
