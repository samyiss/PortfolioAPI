using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioApi.Models
{
    [Table("gallery")]
    public class Gallery
    {
        public int gallery_id { get; set; }
        public required string picture_url { get; set; }
        public string description { get; set; } = string.Empty;
        public string picture_date { get; set; } = string.Empty;
    }
}
