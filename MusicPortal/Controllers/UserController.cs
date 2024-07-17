using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.DataBase;
using MusicPortal.Models.Repository;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Controllers
{
    public class UserController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicRepository _musicRepository;

        public UserController(IGenreRepository genreRepository, IMusicRepository musicRepository)
        {
            _genreRepository = genreRepository;
            _musicRepository = musicRepository;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepository.GetAllGenres();
            var music = await _musicRepository.GetAllMusic();
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
    }
}
