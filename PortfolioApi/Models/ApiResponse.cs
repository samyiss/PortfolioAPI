using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PortfolioApi.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Project>? ListObjects { get; set; }
    }
}
