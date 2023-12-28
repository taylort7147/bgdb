using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class LoginCredentials
{
    [Required]
    public string Username { get; set; } = "";
    
    [Required]
    public string Password { get; set; } = "";
}
