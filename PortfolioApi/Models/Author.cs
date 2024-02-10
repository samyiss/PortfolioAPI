using System.Text.Json.Serialization;

namespace PortfolioApi.Models
{
    public class Author()
    {
        public int author_id { get; set; }
        public string? name { get; set; }
        [JsonIgnore] public List<Authors_Projects>? authors_projects { get; }
    }
}
