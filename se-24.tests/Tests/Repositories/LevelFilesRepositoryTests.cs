using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.tests.Tests.Repositories
{
    public class LevelFilesRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public LevelFilesRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task SaveFinderGameLevels_AddsLevelsToDatabase()
        {
            using var context = new AppDbContext(_options);
            var repository = new LevelFilesRepository(context);

            var levels = new List<Level>
            {
                new Level { Id = 1, Difficulty = "easy" },
                new Level { Id = 2, Difficulty = "medium" }
            };

            await repository.SaveFinderGameLevels(levels);

            var storedLevels = await context.FinderLevels.ToListAsync();
            Assert.Equal(2, storedLevels.Count);
            Assert.Equal("easy", storedLevels[0].Difficulty);
            Assert.Equal("medium", storedLevels[1].Difficulty);
        }

        [Fact]
        public async Task SaveReadingGameLevels_AddsLevelsToDatabase()
        {
            using var context = new AppDbContext(_options);
            var repository = new LevelFilesRepository(context);

            var readingLevels = new List<ReadingLevel>
            {
                new ReadingLevel { Id = 1, Level = 1 },
                new ReadingLevel { Id = 2, Level = 2 }
            };

            await repository.SaveReadingGameLevels(readingLevels);

            var storedLevels = await context.ReadingLevels.ToListAsync();
            Assert.Equal(2, storedLevels.Count);
            Assert.Equal(1, storedLevels[0].Level);
            Assert.Equal(2, storedLevels[1].Level);
        }

        [Fact]
        public async Task SaveFinderGameLevels_ThrowsException_WhenNullLevelsProvided()
        {
            using var context = new AppDbContext(_options);
            var repository = new LevelFilesRepository(context);

            await Assert.ThrowsAsync<NullReferenceException>(() => repository.SaveFinderGameLevels(null));
        }

        [Fact]
        public async Task SaveReadingGameLevels_ThrowsException_WhenNullLevelsProvided()
        {
            using var context = new AppDbContext(_options);
            var repository = new LevelFilesRepository(context);

            await Assert.ThrowsAsync<NullReferenceException>(() => repository.SaveReadingGameLevels(null));
        }
    }
}
