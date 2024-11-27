using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Repositories;
using se_24.shared.src.Shared;

namespace se_24.tests.Tests.Repositories
{
    public class ScoreRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public ScoreRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetScores_ReturnsAllScores()
        {
            using var context = new AppDbContext(_options);
            context.Scores.AddRange(
                new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 },
                new Score { Id = 2, PlayerName = "Player2", GameName = "ReadingGame", Value = 200 }
            );
            await context.SaveChangesAsync();

            var repository = new ScoreRepository(context);

            var scores = await repository.GetScores();

            Assert.Equal(2, scores.Count());
        }

        [Fact]
        public async Task GetScoresByGameName_ReturnsMatchingScores()
        {
            using var context = new AppDbContext(_options);
            context.Scores.AddRange(
                new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 },
                new Score { Id = 2, PlayerName = "Player2", GameName = "ReadingGame", Value = 200 }
            );
            await context.SaveChangesAsync();

            var repository = new ScoreRepository(context);

            var game1Scores = await repository.GetScoresByGameName("FinderGame");

            Assert.Single(game1Scores);
            Assert.Equal("Player1", game1Scores.First().PlayerName);
        }

        [Fact]
        public async Task GetScoresByGameName_NoMatches_ReturnsEmptyList()
        {
            using var context = new AppDbContext(_options);
            context.Scores.AddRange(
                new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 }
            );
            await context.SaveChangesAsync();

            var repository = new ScoreRepository(context);

            var game3Scores = await repository.GetScoresByGameName("NotRealGame");

            Assert.Empty(game3Scores);
        }

        [Fact]
        public async Task GetScoreById_ValidId_ReturnsScore()
        {
            using var context = new AppDbContext(_options);
            var score = new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 };
            context.Scores.Add(score);
            await context.SaveChangesAsync();

            var repository = new ScoreRepository(context);

            var retrievedScore = await repository.GetScoreById(1);

            Assert.NotNull(retrievedScore);
            Assert.Equal("Player1", retrievedScore.PlayerName);
        }

        [Fact]
        public async Task GetScoreById_InvalidId_ThrowsException()
        {
            using var context = new AppDbContext(_options);
            var repository = new ScoreRepository(context);

            await Assert.ThrowsAsync<Exception>(() => repository.GetScoreById(999));
        }

        [Fact]
        public async Task SaveScore_ValidScore_AddsScoreToDatabase()
        {
            using var context = new AppDbContext(_options);
            var repository = new ScoreRepository(context);
            var newScore = new Score { PlayerName = "Player1", GameName = "Game1", Value = 100 };

            await repository.SaveScore(newScore);

            var scores = await context.Scores.ToListAsync();
            Assert.Single(scores);
            Assert.Equal("Player1", scores.First().PlayerName);
        }

        [Fact]
        public async Task SaveScore_NullScore_ThrowsException()
        {
            using var context = new AppDbContext(_options);
            var repository = new ScoreRepository(context);

            await Assert.ThrowsAsync<Exception>(() => repository.SaveScore(null));
        }

        [Theory]
        [InlineData("Player1", null, 100)]
        [InlineData(null, "FinderGame", 100)]
        [InlineData("Player1", "FinderGAme", -10)]
        public async Task SaveScore_InvalidScoreData_ThrowsException(string playerName, string gameName, int value)
        {
            using var context = new AppDbContext(_options);
            var repository = new ScoreRepository(context);
            var invalidScore = new Score { GameName = gameName, PlayerName = playerName, Value = value };

            await Assert.ThrowsAsync<Exception>(() => repository.SaveScore(invalidScore));
        }
    }
}
