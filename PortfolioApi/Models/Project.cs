﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PortfolioApi.Models
{
    public class Project
    {
        public int project_id { get; set; }
        public required string name { get; set; }
        public required string description { get; set; }
        public required string learning { get; set; }
        public required string challenges { get; set; }
        [NotMapped] public List<Author>? authors { get; set; }
        [NotMapped] public List<Tag>? tags { get; set; }
        #pragma warning disable CS8618
        [JsonIgnore] public List<Tags_Projects> Tags_projects { get; }
        [JsonIgnore] public List<Authors_Projects> authors_projects { get; }
        #pragma warning restore CS8618
        public required List<Picture> pictures { get; set; }
        public required string giturl { get; set; }
        public string? apiurl { get; set; }
    }
}