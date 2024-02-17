using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;
using PortfolioApi.Models.WebApplication5.Controllers;

namespace PortfolioApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GalleryController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // POST api/Gallery
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel files)
    {
        try
        {
            using (_context) {
                var uploadTasks = new List<Task<string>>();

                // Loop through each file in the model
                foreach (var file in files.File)
                {
                    // Upload each file to Firebase Storage
                    var uploadTask = UploadFileToFirebaseStorage(file);
                    uploadTasks.Add(uploadTask);
                }

                // Wait for all uploads to complete
                await Task.WhenAll(uploadTasks);

                var listPictures = new List<Gallery>();
                for (var i = 0; i < files.Description.Count; i++)
                {
                    var picture = new Gallery
                    {
                        picture_url = await uploadTasks[i],
                        description = files.Description[i],
                        picture_date = files.Date[i]
                    };

                    listPictures.Add(picture);
                }

                _context.gallery.AddRange(listPictures);
                _context.SaveChanges();

                // You can use downloadUrl for further processing
                return Ok("Files uploaded successfully");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    private async Task<string> UploadFileToFirebaseStorage(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var auth = new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult("AIzaSyAXFVp4ZF7err4aWNbhhh-NdP9F1dLcCCE")
        };

        var downloadUrl = await new FirebaseStorage("portfolio-5d345.appspot.com")
            .Child("pictures")
            .Child(file.FileName)
            .PutAsync(stream);

        return downloadUrl;
    }

    // GET api/Gallery/{type}
    [HttpGet("{type}")]
    public async Task<ActionResult<List<Gallery>>> GetPicturesByType(string type)
    {
        try
        {
            var pictures = await _context.gallery
                .Where(p => p.description == type)
                .ToListAsync();

            if (pictures == null || pictures.Count == 0)
            {
                return NotFound("Pictures not found");
            }

            return pictures;
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
