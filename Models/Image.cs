using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class Image
{
    public int Id { get; set; }

    [Required()]
    public byte[] ImageData { get; set; } = default!;
}
