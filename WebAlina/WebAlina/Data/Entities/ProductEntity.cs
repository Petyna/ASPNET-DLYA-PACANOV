﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAlina.Data.Entities
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }
        [JsonIgnore] public virtual ICollection<ProductImageEntity>? ProductImages { get; set; }
    }
}
