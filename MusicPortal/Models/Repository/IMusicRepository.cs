using MusicPortal.Models.DataBase;
using MusicPortal.Models.ViewModel;

namespace MusicPortal.Models.Repository
{
    public interface IMusicRepository
    {
        Task<IEnumerable<Music>> GetAllMusic();
        Task AddMusic(MusicViewModel model, byte[] fileData);
        Task UpdateMusic(Music music, byte[] fileData = null);
        Task DeleteMusic(int id);
        Task<Music> GetMusicById(int id);
    }
}
