using ContactsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContactsApp.Controllers
{    
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            TempData["ErrorMessage"] = "Invalid email or password.";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "admin,basic")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl
            };
            return View(model);
        }

        [Authorize(Roles = "admin,basic")]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Actualizamos nombre e imagen
            user.FullName = model.FullName;

            // TODO: subir imagen si model.ProfileImageFile != null y obtener URL, aquí solo simulamos
            if (model.ProfileImageFile != null)
            {
                // Aquí deberías implementar subida a disco o cloud y asignar la URL resultante
                // Por ejemplo:
                // user.ProfileImageUrl = await UploadImageAsync(model.ProfileImageFile);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            // Cambiar contraseña si el usuario ingresó datos
            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword))
            {
                var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePassResult.Succeeded)
                {
                    foreach (var error in changePassResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
