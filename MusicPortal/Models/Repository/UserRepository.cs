using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MusicPortalContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(MusicPortalContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterUser(string email, string username, string password)
        {
            var user = new User
            {
                Email = email,
                Name = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                IsConfirmed = false,
                IsAdmin = false,
                IsBlocked = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> AuthorizeUser(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    IsEssential = true
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);

                Console.WriteLine($"User ID {user.Id} set in cookie.");
                return user;
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
