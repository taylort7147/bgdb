using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models
{
    public class BoardGameUserData
    {
        public int Id { get; set; }

        public string? Location { get; set; }

        public BoardGame BoardGame { get; set; }  = default!;

        public User User { get; set; }  = default!;
    }
}
