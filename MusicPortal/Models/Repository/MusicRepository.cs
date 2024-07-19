using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Models.Repository
{
    public class MusicRepository : IMusicRepository
    {
        private readonly MusicPortalContext _context;
        private readonly string _musicFolderPath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MusicRepository(MusicPortalContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _musicFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Music");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Music>> GetAllMusic()
        {
            return await _context.Music.ToListAsync();
        }

        public async Task AddMusic(MusicViewModel model, byte[] fileData)
        {
            var userIdStr = _httpContextAccessor.HttpContext.Request.Cookies["UserId"];
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                Console.WriteLine("User ID is null or invalid in cookie.");
                throw new UnauthorizedAccessException("User is not logged in.");
            }
            Console.WriteLine($"User ID {userId} retrieved from cookie.");

            var filePath = Path.Combine(_musicFolderPath, model.MusicFile.FileName);
            await File.WriteAllBytesAsync(filePath, fileData);
            var music = new Music
            {
                Title = model.Title,
                Artist = model.Artist,
                GenreId = model.GenreId,
                FilePath = filePath,
                UserId = userId
            };

            _context.Music.Add(music);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMusic(Music model, byte[] fileData = null)
        {
            var userIdStr = _httpContextAccessor.HttpContext.Request.Cookies["UserId"];
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                Console.WriteLine("User ID is null or invalid in cookie.");
                throw new UnauthorizedAccessException("User is not logged in.");
            }
            Console.WriteLine($"User ID {userId} retrieved from cookie.");

            var music = await _context.Music.FindAsync(model.Id);
            if (music == null)
            {
                Console.WriteLine($"Music with ID {model.Id} not found.");
                throw new KeyNotFoundException($"Music with ID {model.Id} not found.");
            }

            music.Title = model.Title;
            music.Artist = model.Artist;
            music.GenreId = model.GenreId;
            music.UserId = userId;

            if (fileData != null)
            {
                var filePath = Path.Combine(_musicFolderPath, model.MusicFile.FileName);
                if (!Directory.Exists(_musicFolderPath))
                {
                    Directory.CreateDirectory(_musicFolderPath);
                }

                await File.WriteAllBytesAsync(filePath, fileData);
                music.FilePath = filePath;
            }

            _context.Music.Update(music);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMusic(int id)
        {
            var music = await _context.Music.FindAsync(id);
            if (music != null)
            {
                if (System.IO.File.Exists(music.FilePath))
                {
                    System.IO.File.Delete(music.FilePath);
                }

                _context.Music.Remove(music);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Music> GetMusicById(int id)
        {
            return await _context.Music.FindAsync(id);
        }
    }
}
