using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PortfolioApi.Models
{
    public class Project
    {
        [Required(ErrorMessage = "Please provide a valid project ID")]
        public int project_id { get; set; }
        public required string name { get; set; }
        public required string date_project { get; set; }

        public required string description { get; set; }
        [NotMapped] public List<Author>? authors { get; set; }
        [NotMapped] public List<Tag>? tags { get; set; }
        #pragma warning disable CS8618
        [JsonIgnore] public List<Tags_Projects>? Tags_projects { get; }
        [JsonIgnore] public List<Authors_Projects>? authors_projects { get; }
        #pragma warning restore CS8618
        public required List<Picture> pictures { get; set; }
        public required string giturl { get; set; }
        public string? apiurl { get; set; }
    }
}
