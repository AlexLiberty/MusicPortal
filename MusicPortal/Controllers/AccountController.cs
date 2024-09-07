using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.Repository;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (!await _userRepository.UserExists(model.Email))
                {
                    ModelState.AddModelError("", "Email not found!");
                    return View(model);
                }

                var user = await _userRepository.AuthorizeUser(model.Email, model.Password);
                if (user != null)
                {
                    if (user.IsConfirmed)
                    {
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("UserRole", user.IsAdmin ? "Admin" : "User");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Account is not confirmed.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                if (await _userRepository.UserExists(model.Email))
                {
                    ModelState.AddModelError("", "Email already registered.");
                    return View(model);
                }

                await _userRepository.RegisterUser(model.Email, model.Name, model.Password);
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
