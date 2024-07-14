using MusicPortal.Models.DataBase;

namespace MusicPortal.Models.Repository
{
    public interface IGenreRepository
    {
        Task<bool> AddGenre(string name);
        Task UpdateGenre(Genre genre);
        Task DeleteGenre(int id);
        Task<Genre> GetGenreById(int id);
        Task<IEnumerable<Genre>> GetAllGenres();
    }
}
