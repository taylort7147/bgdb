using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class Image
{
    public int Id { get; set; }

    [Required]
    public byte[] ImageData { get; set; } = default!;

    [Required]
    public string Filename { get; set; } = default!;

    [Required]
    public string Checksum { get; set; } = default!;

    [NotMapped]
    public string MimeType
    {
        get
        {
            var type = Path.GetExtension(Filename).ToLower();
            return $"image/{type}";
        }
    }

}
