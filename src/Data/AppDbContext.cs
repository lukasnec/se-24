using Microsoft.EntityFrameworkCore;
using src.Games.FinderGame;
using src.Games.ReadingGame;
using src.Shared;

namespace src.Data
{
    public class AppDbContext: DbContext
    {
        // Add a DbSet for each entity that needs a table
        public DbSet<ReadingLevel> ReadingLevels { get; set; }
        public DbSet<ReadingQuestion> ReadingQuestions { get; set; }
        public DbSet<Level> FinderLevels { get; set; }
        public DbSet<GameObject> FinderLevelGameObjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Should be configurable in the ideal case
            var connectionString = "Server=127.0.0.1;Port=5432;Database=App;User Id=admin;Password=admin;";

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameObject>()
                        .OwnsOne(g => g.Position);

            base.OnModelCreating(modelBuilder);
        }
    }
}
