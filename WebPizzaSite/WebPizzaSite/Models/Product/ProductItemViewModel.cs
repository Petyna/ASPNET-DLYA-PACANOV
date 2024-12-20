﻿namespace WebPizzaSite.Models.Product
{
    public class ProductItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public List<string>? Images { get; set; }
    }
}
