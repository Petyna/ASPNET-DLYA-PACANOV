using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPizzaSite.Data;
using WebPizzaSite.Data.Entities;
using WebPizzaSite.Models.Product;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using System.IO;

namespace WebPizzaSite.Controllers
{
    
    public class ProductController : Controller
    {
        private readonly PizzaDbContext _pizzaDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(PizzaDbContext pizzaDbContext, IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _pizzaDbContext = pizzaDbContext;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(ProductSearchViewModel search)
        {
            var query = _pizzaDbContext.Products.AsQueryable();
            int pageSize = 8; // Number of products per page
            int page = search.Page ?? 1;
            page = page - 1;

            if (!string.IsNullOrEmpty(search.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.Name.ToLower()));
            }

            int count = query.Count(); // Total number of products

            query = query.OrderBy(x => x.Name).Skip(page * pageSize).Take(pageSize);
            var list = query
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            ProductsHomeViewModel model = new ProductsHomeViewModel()
            {
                Data = list,
                Count = count,
                Pagination = new Models.Helpers.PaginationViewModel
                {
                    PageSize = pageSize,
                    TotalItems = count
                },
                Search = new ProductSearchViewModel
                {
                    Name = search.Name,
                    Page = search.Page ?? 1
                }
            };

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var catList = _pizzaDbContext.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            ProductCreateViewModel model = new ProductCreateViewModel();
            model.CategoryList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(catList, "Value", "Text");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var prod = new ProductEntity
            {
                Name = model.Name,
                Price = model.Price,
                CategoryId = model.CategoryId,
            };

            await _pizzaDbContext.Products.AddAsync(prod);
            await _pizzaDbContext.SaveChangesAsync();

            if (model.Photos != null)
            {
                int i = 0;

                foreach (var img in model.Photos)
                {
                    string ext = ".webp"; // Змінюємо розширення на webp
                    string fileName = Guid.NewGuid().ToString() + ext;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                    // Використовуємо ImageSharp для обробки зображення
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(img.OpenReadStream()))
                    {
                        // Змінюємо розмір зображення
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(512, 512), // Задайте бажаний розмір
                            Mode = ResizeMode.Max // Режим масштабування
                        }));

                        // Збереження зображення у форматі WebP
                        await image.SaveAsync(path, new WebpEncoder());
                    }

                    var imgEntity = new ProductImageEntity
                    {
                        Name = fileName,
                        Priority = i++,
                        Product = prod,
                    };
                    _pizzaDbContext.ProductImages.Add(imgEntity);
                    await _pizzaDbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
