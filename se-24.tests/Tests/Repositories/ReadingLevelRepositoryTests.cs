using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace se_24.tests.Tests.Repositories
{
    public class ReadingLevelRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public ReadingLevelRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetReadingGameLevels_ReturnsAllLevels()
        {
            using var context = new AppDbContext(_options);
            context.ReadingLevels.AddRange(
                new ReadingLevel { Id = 1, Level = 1 },
                new ReadingLevel { Id = 2, Level = 2 }
            );
            await context.SaveChangesAsync();

            var repository = new ReadingLevelRepository(context);

            var levels = await repository.GetReadingGameLevels();

            Assert.Equal(2, levels.Count());
        }

        [Fact]
        public async Task GetReadingGameLevelsByReadingLevel_ReturnsMatchingLevels()
        {
            using var context = new AppDbContext(_options);
            context.ReadingLevels.AddRange(
                new ReadingLevel { Id = 1, Level = 1 },
                new ReadingLevel { Id = 2, Level = 2 }
            );
            await context.SaveChangesAsync();

            var repository = new ReadingLevelRepository(context);

            var levels = await repository.GetReadingGameLevelsByReadingLevel(1);

            Assert.Single(levels);
            Assert.Equal(1, levels.First().Level);
        }

        [Fact]
        public async Task GetFinderLevelsByDifficulty_NoMatches_ReturnsEmptyList()
        {
            using var context = new AppDbContext(_options);
            context.ReadingLevels.AddRange(
                new ReadingLevel { Id = 1, Level = 1 }
            );
            await context.SaveChangesAsync();

            var repository = new ReadingLevelRepository(context);

            var levels = await repository.GetReadingGameLevelsByReadingLevel(99);

            Assert.Empty(levels);
        }
    }
}
