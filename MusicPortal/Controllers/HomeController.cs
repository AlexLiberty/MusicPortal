using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models;
using MusicPortal.Models.Repository;
using System.Diagnostics;
using System.Globalization;

namespace MusicPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IGenreRepository _genreRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IMusicRepository musicRepository, IGenreRepository genreRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _musicRepository = musicRepository;
            _genreRepository = genreRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["CurrentCulture"] = CultureInfo.CurrentUICulture.Name;

            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            var genres = await _genreRepository.GetAllGenres();
            var music = await _musicRepository.GetAllMusic();

            if (int.TryParse(userIdString, out var userId))
            {
                var user = await _userRepository.GetUserById(userId);
                if (user != null)
                {
                    ViewData["UserName"] = user.Name;
                    ViewData["IsAdmin"] = userRole == "Admin";
                }
            }

            ViewData["Genres"] = genres;
            ViewData["Music"] = music;

            return View(music);
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

        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            Console.WriteLine($"Culture set to: {culture}");
            Console.WriteLine($"Cookies: {Request.Cookies[CookieRequestCultureProvider.DefaultCookieName]}");

            return Json(new { success = true });
        }
    }
}
