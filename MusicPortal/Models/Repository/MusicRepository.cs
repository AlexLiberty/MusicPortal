using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;

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
        }

        public async Task<IEnumerable<Music>> GetAllMusic()
        {
            return await _context.Music.ToListAsync() ?? Enumerable.Empty<Music>();
        }

        public async Task AddMusic(Music music, byte[] fileData)
        {
            var filePath = Path.Combine(_musicFolderPath, music.MusicFile.FileName);

            await File.WriteAllBytesAsync(filePath, fileData);

            music.FilePath = filePath;

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
    }
}
