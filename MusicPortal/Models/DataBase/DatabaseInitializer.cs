namespace MusicPortal.Models.DataBase
{
    public class DatabaseInitializer
    {
        public static void Initialize(MusicPortalContext context)
        {
            if (context.Database.EnsureCreated())
            {
                var salt = SecurityHelper.GenerateSalt(16);
                context.Users.Add(new User
                {
                    Email = "admin@mail.com",
                    Name = "Admin",
                    Password = SecurityHelper.HashPassword("Admin", salt, 10000, 32),
                    Salt = salt,
                    IsConfirmed = true,
                    IsAdmin = true,
                    IsBlocked = false
                });
                context.SaveChanges();
            }
        }
    }
}
