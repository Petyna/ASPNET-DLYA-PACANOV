using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using WebPizzaSite.Data.Entities.Identity;
using WebPizzaSite.Models.Account;

namespace WebPizzaSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;

        public AccountController(UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null) 
            {
                var res = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

                if (res.Succeeded) 
                { 
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect("/");
                }
            }

            ModelState.AddModelError("", "Дані вказано не вірно!");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new LoginViewModel
            {
                Email = user.Email,
                UserName = user.UserName,  
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();  // This will load the Register.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // Return the same view if the model is invalid
            }

            var user = new UserEntity { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Profile", "Account");  // Redirect to Profile after successful registration
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);  // Reload the form with errors if registration fails
        }

    }


}
