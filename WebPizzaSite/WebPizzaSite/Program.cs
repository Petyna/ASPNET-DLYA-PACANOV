using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebPizzaSite.Constants;
using WebPizzaSite.Data;
using WebPizzaSite.Data.Entities;
using WebPizzaSite.Data.Entities.Identity;
using WebPizzaSite.Interfaces;
using WebPizzaSite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IImageWorker, ImageWorker>();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PizzaDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity options
builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<PizzaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Seed data
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<PizzaDbContext>();
    var userManager = serviceScope.ServiceProvider.GetService<UserManager<UserEntity>>();
    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<RoleEntity>>();
    var imageWorker = serviceScope.ServiceProvider.GetService<IImageWorker>();

    context?.Database.Migrate();

    // Seed Categories
    if (!context.Categories.Any())
    {
        string url = "https://loremflickr.com/1200/800/tokio,cat/all";
        var faker = new Faker("uk");
        var categories = faker.Commerce.Categories(10);
        foreach (var categoryName in categories)
        {
            string fileName = imageWorker.ImageSave(url);
            if (!string.IsNullOrEmpty(fileName))
            {
                var entity = new CategoryEntity
                {
                    Name = categoryName,
                    Description = faker.Lorem.Lines(5),
                    Image = fileName
                };
                context.Categories.Add(entity);
            }
        }
        context.SaveChanges();
    }

    // Seed Products
    if (!context.Products.Any())
    {
        string url = "https://loremflickr.com/1200/800/car/all";
        var faker = new Faker("uk");
        var catCount = context.Categories.Count();
        if (catCount > 0)
        {
            var catIds = context.Categories.Select(x => x.Id).ToList();
            int productCount = 100;
            for (int k = 0; k < productCount; k++)
            {
                var catIndex = faker.Random.Number(0, catCount - 1);
                var p = new ProductEntity
                {
                    CategoryId = catIds[catIndex],
                    Name = faker.Commerce.ProductName(),
                    Price = decimal.Parse(faker.Commerce.Price())
                };
                context.Add(p);
                int countImages = faker.Random.Number(3, 5);
                for (int i = 0; i < countImages; i++)
                {
                    string fileName = imageWorker.ImageSave(url);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var pi = new ProductImageEntity
                        {
                            Name = fileName,
                            Priority = i,
                            Product = p
                        };
                        context.Add(pi);
                    }
                }
                context.SaveChanges();
            }
        }
    }

    // Seed Roles
    if (!context.Roles.Any())
    {
        var adminRole = new RoleEntity { Name = Roles.Admin };
        var userRole = new RoleEntity { Name = Roles.User };

        await roleManager.CreateAsync(adminRole);
        await roleManager.CreateAsync(userRole);
    }

    // Seed Admin User
    if (!context.Users.Any())
    {
        var user = new UserEntity
        {
            Email = "admin@gmail.com",
            UserName = "admin@gmail.com",
            LastName = "Шолом",
            FirstName = "Вулкан",
            Picture = "admin.jpg"
        };
        var result = await userManager.CreateAsync(user, "123456");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "admin_area",
        areaName: "Admin",
        pattern: "/admin/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Main}/{action=Index}/{id?}");
});

app.Run();
