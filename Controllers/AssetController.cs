using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BggExt.Controllers;

[ApiController]
[Route("api/asset")]
public class AssetController(BoardGameDbContext _context) : ControllerBase
{   
    [AllowAnonymous]
    [HttpGet("img/{id}")]
    public async Task<IActionResult> GetImage(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if(image == null)
        {
            return NotFound();
        }
        return File(image.ImageData, image.MimeType);
    }
}
