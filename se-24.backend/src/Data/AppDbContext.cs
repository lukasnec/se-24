using Microsoft.EntityFrameworkCore;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using se_24.shared.src.Shared;

namespace se_24.backend.src.Data
{
    public class AppDbContext : DbContext
    {
        // Add a DbSet for each entity that needs a table
        public DbSet<ReadingLevel> ReadingLevels { get; set; }
        public DbSet<ReadingQuestion> ReadingQuestions { get; set; }
        public DbSet<Level> FinderLevels { get; set; }
        public DbSet<GameObject> FinderLevelGameObjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReadingLevel>().HasKey(rl => rl.Id);
            modelBuilder.Entity<ReadingQuestion>().HasKey(rq => rq.Id);
            modelBuilder.Entity<GameObject>().HasKey(go => go.Id);
            modelBuilder.Entity<Level>().HasKey(l => l.Id);
            modelBuilder.Entity<GameObject>()
                        .OwnsOne(g => g.Position);

            base.OnModelCreating(modelBuilder);
        }
    }
}
