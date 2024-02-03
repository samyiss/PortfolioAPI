using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioApi.Models
{
    [Table("authors_projects")]
    public class Authors_Projects
    {
        public required int project_id { get; set; }
        public required int author_id { get; set; }
        public required Author authors { get; set; }
        public required Project projects { get; set; }
    }
}
