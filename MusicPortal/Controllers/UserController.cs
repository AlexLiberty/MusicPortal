using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MusicPortal.Hubs;
using MusicPortal.Models.DataBase;
using MusicPortal.Models.Repository;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Controllers
{
    public class UserController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicRepository _musicRepository;
        private readonly IUserRepository _userRepository;
        IHubContext<NotificationHub> _hubContext {  get; }

        public UserController(IGenreRepository genreRepository, IMusicRepository musicRepository, IUserRepository userRepository, IHubContext<NotificationHub> hubContext)
        {
            _genreRepository = genreRepository;
            _musicRepository = musicRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepository.GetAllGenres();
            var music = await _musicRepository.GetAllMusicUserAdmin();
            var users = await _userRepository.GetAllUsers();
            ViewData["Genres"] = genres;
            ViewData["Music"] = music;

            return View(music);
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
                    await SendMessage("Додана нова пісня: " + model.Artist + " " + model.GenreId + " " + model.Title);
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
            var music = await _musicRepository.GetAllMusicUserAdmin();

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

        private async Task SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("displayMessage", message);
        }
    }
}
