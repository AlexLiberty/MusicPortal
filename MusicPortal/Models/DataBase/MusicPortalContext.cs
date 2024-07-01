using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.DataBase
{
    public class MusicPortalContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public MusicPortalContext(DbContextOptions<MusicPortalContext> options) : base(options){}
    }
}
