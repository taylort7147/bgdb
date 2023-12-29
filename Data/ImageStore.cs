using BggExt.Data;
using BggExt.Models;
using BggExt.Web;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Cryptography;

/// <summary>
/// A class to manage storing images for hosting.
/// </summary>
/// <param name="_logger"></param>
/// <param name="_scopeFactory"></param>
/// <param name="_downloader">The downloader used to pull images from URLs</param>
public class ImageStore(
    ILogger<ImageStore> _logger,
    IServiceScopeFactory _scopeFactory,
    Downloader _downloader)
{
    private string ComputeChecksum(byte[] data)
    {
        SHA256 sha = SHA256.Create();
        var checksum = sha.ComputeHash(data);
        return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
    }

    public async Task<Image> StoreImage(Uri uri)
    {
        var filename = Path.GetFileName(uri.LocalPath);
        var imageData = await _downloader.DownloadByteArray(uri);
        var image = new Image()
        {
            ImageData = imageData,
            Filename = filename,
            Checksum = ComputeChecksum(imageData)
        };

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
            var existingImage = await context.Images
                .Where(i => i.Filename == image.Filename && i.Checksum == image.Checksum)
                .FirstOrDefaultAsync();
            if (existingImage != null)
            {
                _logger.LogInformation(
                    $"Image '{image.Filename}' with checksum '{image.Checksum}' already exists in the database. Returning a reference to the existing image.");
                return existingImage;
            }
            context.Images.Add(image);
            await context.SaveChangesAsync();
        }

        return image;
    }
}
