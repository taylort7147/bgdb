using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BggExt.Models;

public class ApplicationUser : IdentityUser
{
    [ForeignKey("Libraries")]
    [Required]
    public virtual Library Library { get; set; } = default!;
}
