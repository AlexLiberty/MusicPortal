using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MusicPortalContext _context;

        public UserRepository(MusicPortalContext context)
        {
            _context = context;
        }

        public async Task RegisterUser(string email, string username, string password)
        {
            string salt = SecurityHelper.GenerateSalt(16);
            string hashedPassword = SecurityHelper.HashPassword(password, salt, 10000, 32);

            var user = new User
            {
                Email = email,
                Name = username,
                Password = hashedPassword,
                Salt = salt,
                IsConfirmed = false,
                IsAdmin = false,
                IsBlocked=false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> AuthorizeUser(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user != null && user.IsConfirmed)
            {
                string hashedPassword = SecurityHelper.HashPassword(password, user.Salt, 10000, 32);
                if (user.Password == hashedPassword)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task<IEnumerable<User>> GetUnconfirmedUsers()
        {
            return await _context.Users.Where(u => !u.IsConfirmed).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.Where(u => !u.IsAdmin).ToListAsync();
        }

        public async Task ConfirmUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsConfirmed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task BlockUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsBlocked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnblockUser(int userId) 
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsBlocked = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
