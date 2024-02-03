using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioApi.Models
{
    [Table("tags_projects")]
    public class Tags_Projects
    {
        public required int project_id { get; set; }
        public required int tag_id { get; set; }
        public required Tag  tags { get; set; }
        public required Project projects { get; set; }
    }
}
