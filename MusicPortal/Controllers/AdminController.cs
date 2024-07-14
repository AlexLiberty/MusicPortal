using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.DataBase;
using MusicPortal.Models.Repository;

namespace MusicPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicRepository _musicRepository;

        public AdminController(IUserRepository userRepository, IGenreRepository genreRepository, IMusicRepository musicRepository)
        {
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _musicRepository = musicRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            var nonAdminUsers = users.Where(u => !u.IsAdmin);
            var confirmedUsers = users.Where(u => u.IsConfirmed && !u.IsAdmin);

            var genres = await _genreRepository.GetAllGenres();
            var music = await _musicRepository.GetAllMusic();

            ViewData["NonAdminUsers"] = nonAdminUsers;
            ViewData["ConfirmedUsers"] = confirmedUsers;
            ViewData["Genres"] = genres;
            ViewData["Music"] = music;

            return View();
        }

        public async Task<IActionResult> UserConfirmationPartial()
        {
            var users = await _userRepository.GetAllUsers();
            var unconfirmedUsers = users.Where(u => !u.IsConfirmed);
            return PartialView("UserConfirmation", unconfirmedUsers);
        }

        public async Task<IActionResult> UserManagementPartial()
        {
            var users = await _userRepository.GetAllUsers();
            var confirmedUsers = users.Where(u => u.IsConfirmed && !u.IsAdmin);
            return PartialView("UserManagement", confirmedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(int userId)
        {
            await _userRepository.ConfirmUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(int userId)
        {
            await _userRepository.BlockUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            await _userRepository.UnblockUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userRepository.DeleteUser(userId);
            return Json(new { success = true });
        }

        public async Task<IActionResult> GenreManagementPartial()
        {
            var genres = await _genreRepository.GetAllGenres();
            return PartialView("GenreManagement", genres);
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(string name)
        {
            bool isAdded = await _genreRepository.AddGenre(name);

            if (isAdded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditGenre(Genre genre)
        {
            await _genreRepository.UpdateGenre(genre);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            await _genreRepository.DeleteGenre(id);
            return Json(new { success = true });
        }

        public async Task<IActionResult> MusicManagementPartial()
        {
            var music = await _musicRepository.GetAllMusic();
            ViewBag.Genres = await _genreRepository.GetAllGenres();
            return PartialView("MusicManagement", music);
        }

        [HttpPost]
        public async Task<IActionResult> AddMusic(Music music)
        {
            byte[] fileData = null;

            if (music.MusicFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await music.MusicFile.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }
            }

            await _musicRepository.AddMusic(music, fileData);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditMusic(Music music)
        {
            byte[] fileData = null;

            if (music.MusicFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await music.MusicFile.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }
            }

            await _musicRepository.UpdateMusic(music, fileData);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMusic(int id)
        {
            await _musicRepository.DeleteMusic(id);
            return Json(new { success = true });
        }
    }
}
