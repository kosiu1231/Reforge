namespace Reforge.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Mod> Mods => Set<Mod>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Like> Likes => Set<Like>();
    }
}
