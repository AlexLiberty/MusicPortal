using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Models.Repository
{
    public class MusicRepository : IMusicRepository
    {
        private readonly MusicPortalContext _context;
        private readonly string _musicFolderPath;

        public MusicRepository(MusicPortalContext context)
        {
            _context = context;
            _musicFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Music");
            if (!Directory.Exists(_musicFolderPath))
            {
                Directory.CreateDirectory(_musicFolderPath);
            }
        }

        public async Task<IEnumerable<Music>> GetAllMusic()
        {
            return await _context.Music.ToListAsync();
        }

        public async Task AddMusic(MusicViewModel model, byte[] fileData)
        {
            var filePath = Path.Combine(_musicFolderPath, model.MusicFile.FileName);
            await File.WriteAllBytesAsync(filePath, fileData);

            var music = new Music
            {
                Title = model.Title,
                Artist = model.Artist,
                GenreId = model.GenreId,
                FilePath = filePath
            };

            _context.Music.Add(music);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMusic(Music music, byte[] fileData = null)
        {
            if (fileData != null)
            {
                var filePath = Path.Combine(_musicFolderPath, music.MusicFile.FileName);

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
