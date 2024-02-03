using System.Text.Json.Serialization;

namespace PortfolioApi.Models
{
    public class Tag
    {
        public required int tag_id { get; set; }
        public required string name { get; set; }
        [JsonIgnore] public List<Tags_Projects>? tags_projects { get; }
    }
}
