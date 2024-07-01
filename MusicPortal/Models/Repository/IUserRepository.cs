using MusicPortal.Models.DataBase;

namespace MusicPortal.Models.Repository
{
    public interface IUserRepository
    {
        Task RegisterUser(string email, string username, string password);
        Task<User> AuthorizeUser(string email, string password);
        Task<bool> UserExists(string email);
        Task<User> GetUserById(int userId);
        Task<IEnumerable<User>> GetUnconfirmedUsers();
        Task ConfirmUser(int userId);
        Task BlockUser(int userId);
        Task UnblockUser(int userId);
        Task DeleteUser(int userId);
        Task<IEnumerable<User>> GetAllUsers();
    }
}
