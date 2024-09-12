using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.DataBase
{
    public class MusicPortalContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Music> Music { get; set; }
        private readonly IConfiguration _configuration;
        public MusicPortalContext(DbContextOptions<MusicPortalContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;

            if (Database.EnsureCreated())
            {
                var adminEmail = _configuration["AdminCredentials:Email"];
                var adminName = _configuration["AdminCredentials:Name"];
                var adminPassword = _configuration["AdminCredentials:Password"];

                Users.Add(new User
                {
                    Email = adminEmail,
                    Name = adminName,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    IsConfirmed = true,
                    IsAdmin = true,
                    IsBlocked = false
                });
                SaveChanges();
            }
        }
    }
}
