﻿using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.DataBase;
using MusicPortal.Models.Repository;
using MusicPortal.Models.ViewModel;

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
            var music = await _musicRepository.GetAllMusicUserAdmin();

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
        public async Task<IActionResult> AddMusic(MusicViewModel model)
        {
            if (model.MusicFile != null)
            {
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await model.MusicFile.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                try
                {
                    await _musicRepository.AddMusic(model, fileData);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding music: {ex.Message}");
                    return Json(new { success = false, message = "An error occurred while adding the music." });
                }
            }
            return Json(new { success = false, message = "No music file provided." });
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

        public async Task<IActionResult> PlayMusic(int id)
        {
            var music = await _musicRepository.GetMusicById(id);
            if (music == null)
            {
                return NotFound();
            }

            var musicUrl = Url.Content($"/Music/{Path.GetFileName(music.FilePath)}");
            return Json(new { url = musicUrl });
        }

        public async Task<IActionResult> GetSortedMusic(string column, string order)
        {
            var music = await _musicRepository.GetAllMusic();
            var genres = await _genreRepository.GetAllGenres();

            switch (column.ToLower())
            {
                case "title":
                    music = order == "asc" ? music.OrderBy(m => m.Title).ToList() : music.OrderByDescending(m => m.Title).ToList();
                    break;
                case "artist":
                    music = order == "asc" ? music.OrderBy(m => m.Artist).ToList() : music.OrderByDescending(m => m.Artist).ToList();
                    break;
                case "genre":
                    music = order == "asc" ? music.OrderBy(m => m.Genre?.Name).ToList() : music.OrderByDescending(m => m.Genre?.Name).ToList();
                    break;
                default:
                    break;
            }

            return PartialView("MusicTable", music);
        }

        public async Task<IActionResult> GetFilteredMusic(string column, string order, int? genreId, string artist)
        {
            var music = await _musicRepository.GetAllMusic();

            if (genreId.HasValue)
            {
                music = music.Where(m => m.GenreId == genreId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(artist))
            {
                music = music.Where(m => m.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            switch (column.ToLower())
            {
                case "title":
                    music = order == "asc" ? music.OrderBy(m => m.Title).ToList() : music.OrderByDescending(m => m.Title).ToList();
                    break;
                case "artist":
                    music = order == "asc" ? music.OrderBy(m => m.Artist).ToList() : music.OrderByDescending(m => m.Artist).ToList();
                    break;
                case "genre":
                    music = order == "asc" ? music.OrderBy(m => m.Genre?.Name).ToList() : music.OrderByDescending(m => m.Genre?.Name).ToList();
                    break;
                default:
                    break;
            }

            var genres = await _genreRepository.GetAllGenres();
            ViewBag.Genres = genres;

            return PartialView("MusicTable", music);
        }

        public async Task<IActionResult> GetFilteredAndSortedUsers(string column, string order, string status, string nameEmail)
        {
            var users = await _userRepository.GetAllUsers();

            users = users.Where(u => u.IsConfirmed && !u.IsAdmin).ToList();

            if (!string.IsNullOrEmpty(status))
            {
                bool isBlocked = status == "Blocked";
                users = users.Where(u => u.IsBlocked == isBlocked).ToList();
            }

            if (!string.IsNullOrEmpty(nameEmail))
            {
                users = users.Where(u => u.Name.Contains(nameEmail, StringComparison.OrdinalIgnoreCase) ||
                                          u.Email.Contains(nameEmail, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            switch (column.ToLower())
            {
                case "email":
                    users = order == "asc" ? users.OrderBy(u => u.Email).ToList() : users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "name":
                    users = order == "asc" ? users.OrderBy(u => u.Name).ToList() : users.OrderByDescending(u => u.Name).ToList();
                    break;
                case "status":
                    users = order == "asc" ? users.OrderBy(u => u.IsBlocked).ToList() : users.OrderByDescending(u => u.IsBlocked).ToList();
                    break;
                case "registrationdate":
                    users = order == "asc" ? users.OrderBy(u => u.Timestamp).ToList() : users.OrderByDescending(u => u.Timestamp).ToList();
                    break;
                default:
                    break;
            }

            return PartialView("ConfirmedUsersTable", users);
        }
    }
}
