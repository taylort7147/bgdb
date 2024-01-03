using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace BggExt.Models;

public class ApplicationUserRegistration
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [RegularExpression("^[a-zA-Z0-9_@.-]{3,64}$")]
    [DisplayName("Username")]
    public string UserName { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
    
    [Required]
    [Compare(nameof(Password))]
    public string PasswordConfirm { get; set; } = default!;

    [Required]
    [DisplayName("BoardGameGeek Username")]
    public string BoardGameGeekUsername { get; set; } = default!;
}
