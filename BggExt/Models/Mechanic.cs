using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class Mechanic
    {
        public int Id { get; set; }
        
        [Required()]
        public string Name { get; set; } = null!;

       public ICollection<BoardGame> BoardGames = null!;
    }
}