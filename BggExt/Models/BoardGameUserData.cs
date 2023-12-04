using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class BoardGameUserData
    {
        [Required()]
        public BoardGame BoardGame { get; set; } = null!;

        [Required()]
        public User User { get; set; } = null!;

        public string? Location { get; set; }
    }
}
