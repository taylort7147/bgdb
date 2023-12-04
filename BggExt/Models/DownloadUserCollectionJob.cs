using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class DownloadUserCollectionJob
    {
        [Required()]
        public string UserId { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool WasSuccessful { get; set; }
        
        public string? Details { get; set; }
    }
}