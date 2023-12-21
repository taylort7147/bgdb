using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BggExt.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public Library Library { get; set; } = default!;
}
