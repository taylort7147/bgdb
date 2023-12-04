using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class Family
    {
        public int Id { get; set; }
        
        [Required()]
        public string Name { get; set; } = null!;

       public ICollection<BoardGame> BoardGames = null!;
    }
}