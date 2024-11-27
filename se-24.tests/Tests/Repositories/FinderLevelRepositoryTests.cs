using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;

namespace se_24.tests.Tests.Repositories
{
    public class FinderLevelRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public FinderLevelRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetFinderGameLevels_ReturnsAllLevels()
        {
            using var context = new AppDbContext(_options);
            context.FinderLevels.AddRange(
                new Level { Id = 1, Difficulty = "easy" },
                new Level { Id = 2, Difficulty = "medium" }
            );
            await context.SaveChangesAsync();

            var repository = new FinderLevelRepository(context);

            var levels = await repository.GetFinderGameLevels();

            Assert.Equal(2, levels.Count());
        }

        [Fact]
        public async Task GetFinderGameLevelsByDifficulty_ReturnsMatchingLevels()
        {
            using var context = new AppDbContext(_options);
            context.FinderLevels.AddRange(
                new Level { Id = 1, Difficulty = "easy" },
                new Level { Id = 2, Difficulty = "medium" }
            );
            await context.SaveChangesAsync();

            var repository = new FinderLevelRepository(context);

            var levels = await repository.GetFinderGameLevelsByDifficulty("easy");

            Assert.Single(levels);
            Assert.Equal("easy", levels.First().Difficulty);
        }

        [Fact]
        public async Task GetFinderLevelsByDifficulty_NoMatches_ReturnsEmptyList()
        {
            using var context = new AppDbContext(_options);
            context.FinderLevels.AddRange(
                 new Level { Id = 1, Difficulty = "easy" }
            );
            await context.SaveChangesAsync();

            var repository = new FinderLevelRepository(context);

            var levels = await repository.GetFinderGameLevelsByDifficulty("impossible");

            Assert.Empty(levels);
        }
    }
}
