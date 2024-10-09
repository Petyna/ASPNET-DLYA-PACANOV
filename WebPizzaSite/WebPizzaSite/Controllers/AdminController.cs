using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebPizzaSite.Data.Entities.Identity;
using WebPizzaSite.Models.Admin; // Create this model for user management

namespace WebPizzaSite.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only admins can access this
    public class AdminController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;

        public AdminController(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList(); // Get all registered users
            return View(users); // Pass users to the view
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnabled = true; // Enable lockout
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Lock the user indefinitely
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }

        // Add more admin functionalities as needed
    }
}
