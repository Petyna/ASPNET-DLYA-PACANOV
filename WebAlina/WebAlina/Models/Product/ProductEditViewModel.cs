using System.Text.Json.Serialization;

namespace WebAlina.Models.Product
{
    public class ProductEditViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore] public List<IFormFile>? Images { get; set; } // Optional: only if updating images
    }

}
