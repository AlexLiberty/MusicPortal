using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models;
using MusicPortal.Models.Repository;
using System.Diagnostics;

namespace MusicPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            if (int.TryParse(userIdString, out var userId))
            {
                var user = await _userRepository.GetUserById(userId);
                if (user != null)
                {
                    ViewData["UserName"] = user.Name;
                    ViewData["IsAdmin"] = userRole == "Admin";
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
