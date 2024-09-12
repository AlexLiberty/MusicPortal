using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.DataBase
{
    public class MusicPortalContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Music> Music { get; set; }
        public MusicPortalContext(DbContextOptions<MusicPortalContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                Users.Add(new User
                {
                    Email = "admin@mail.com",
                    Name = "Admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin"),
                    IsConfirmed = true,
                    IsAdmin = true,
                    IsBlocked = false
                });
                SaveChanges();
            }
        }
    }
}
