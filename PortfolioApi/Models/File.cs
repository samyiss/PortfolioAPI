using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models
{
    namespace WebApplication5.Controllers
    {
        public class FileUploadViewModel
        {
            [Required]
            public List<IFormFile> File { get; set; }
            [Required]
            public List<string> Description { get; set; }
            [Required]
            public List<string> Date { get; set; }
        }
    }
}
