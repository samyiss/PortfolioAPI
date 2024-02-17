﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PortfolioApi.Models
{
    public class Picture
    {
        [JsonIgnore] public int? picture_id { get; set; }
        public required string picture { get; set; }
        public string description { get; set; } = string.Empty;
        public string date { get; } = string.Empty;
    }
}

