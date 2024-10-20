using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using WebAlina.Data;
using WebAlina.Data.Entities;
using WebAlina.Interfaces;
using WebAlina.Models.Product;

namespace WebAlina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly AlinaDbContext _context;
        public readonly IConfiguration _configuration;
        public readonly IImageHulk _imageHulk;
        public readonly IMapper _mapper;
        public ProductsController(AlinaDbContext context, IConfiguration configuration, 
            IMapper mapper, IImageHulk imageHulk)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _imageHulk = imageHulk;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var list = await _context.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateViewModel model)
        {
            var entity = _mapper.Map<ProductEntity>(model);
            _context.Products.Add(entity);

            if (model.Images != null)
            {
                var p = 1;
                foreach (var image in model.Images)
                {
                    var imageName = await _imageHulk.Save(image);
                    var imageProduct = new ProductImageEntity
                    {
                        Product = entity,
                        Image = imageName,
                        Priority = p++
                    };
                    _context.Add(imageProduct);
                }
            }

            await _context.SaveChangesAsync(); // Зберігаємо всі зміни тут
            return Ok(entity.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
        .Include(p => p.ProductImages) // Ensures images are loaded
        .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Path to the folder where images are stored
            var imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploading");

            // Delete all product images
            foreach (var image in product.ProductImages)
            {
                var path = Path.Combine(imageUploadPath, image.Image); // Form full path
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path); // Delete image file
            }

            // Remove the product from the database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 (No Content)
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductEditViewModel model)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            var imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploading");

            if (model.Images != null && model.Images.Count > 0)
            {
                // Видалити старі зображення
                foreach (var image in product.ProductImages)
                {
                    var path = Path.Combine(imageUploadPath, image.Image);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.ProductImages.RemoveRange(product.ProductImages);

                // Додати нові зображення
                var imagePriority = 1;
                foreach (var newImage in model.Images)
                {
                    var newImageName = await _imageHulk.Save(newImage);
                    var productImage = new ProductImageEntity
                    {
                        Product = product,
                        Image = newImageName,
                        Priority = imagePriority++
                    };

                    _context.ProductImages.Add(productImage);
                }
            }

            await _context.SaveChangesAsync(); // Зберігаємо всі зміни тут
            return Ok(product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products.Include(p => p.ProductImages).SingleOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound(); // Повертаємо 404, якщо продукт не знайдено
                }

                var productViewModel = _mapper.Map<ProductDetailsViewModel>(product);
                return Ok(productViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
