using Microsoft.AspNetCore.Mvc;
using Moq;
using se_24.backend.Controllers;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Shared;

namespace se_24.tests.Tests.Controllers
{
    public class ScoreControllerTests
    {
        private readonly Mock<IScoreRepository> _mockRepo;
        private readonly ScoreController _controller;

        public ScoreControllerTests()
        {
            _mockRepo = new Mock<IScoreRepository>();
            _controller = new ScoreController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetScores_ReturnsListOfScores()
        {
            var mockScores = new List<Score>
            {
                new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 },
                new Score { Id = 2, PlayerName = "Player2", GameName = "ReadingGame", Value = 200 }
            };
            _mockRepo.Setup(repo => repo.GetScores()).ReturnsAsync(mockScores);

            var result = await _controller.GetScores();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedScores = Assert.IsType<List<Score>>(okResult.Value);
            Assert.Equal(2, returnedScores.Count);
            _mockRepo.Verify(repo => repo.GetScores(), Times.Once);
        }

        [Fact]
        public async Task GetScoresByGameName_ReturnsScores_WithCorrectGameName()
        {
            var gameName = "FinderGame";
            var mockScores = new List<Score>
            {
                new Score { Id = 1, PlayerName = "Player1", GameName = gameName, Value = 150 }
            };
            _mockRepo.Setup(repo => repo.GetScoresByGameName(gameName)).ReturnsAsync(mockScores);

            var result = await _controller.GetScoresByGameName(gameName);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedScores = Assert.IsType<List<Score>>(okResult.Value);
            Assert.Single(returnedScores);
            Assert.Equal(gameName, returnedScores[0].GameName);
            _mockRepo.Verify(repo => repo.GetScoresByGameName(gameName), Times.Once);
        }

        [Fact]
        public async Task GetScoreById_ReturnsOkResult_WhenScoreExists()
        {
            var scoreId = 1;
            var mockScore = new Score { Id = scoreId, PlayerName = "Player1", GameName = "FinderGame", Value = 300 };
            _mockRepo.Setup(repo => repo.GetScoreById(scoreId)).ReturnsAsync(mockScore);

            var result = await _controller.GetScoreById(scoreId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedScore = Assert.IsType<Score>(okResult.Value);
            Assert.Equal(scoreId, returnedScore.Id);
            _mockRepo.Verify(repo => repo.GetScoreById(scoreId), Times.Once);
        }

        [Fact]
        public async Task GetScoreById_ReturnsBadRequest_WhenExceptionThrown()
        {
            var scoreId = 99;
            _mockRepo.Setup(repo => repo.GetScoreById(scoreId)).ThrowsAsync(new Exception("Score not found"));

            var result = await _controller.GetScoreById(scoreId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Score not found", badRequestResult.Value);
            _mockRepo.Verify(repo => repo.GetScoreById(scoreId), Times.Once);
        }

        [Fact]
        public async Task SaveScore_ReturnsCreatedAtAction_WhenSaveSuccessful()
        {
            var newScore = new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 };
            _mockRepo.Setup(repo => repo.SaveScore(newScore)).Returns(Task.CompletedTask);

            var result = await _controller.SaveScore(newScore);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ScoreController.GetScoreById), createdAtActionResult.ActionName);
            Assert.Equal(newScore.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(newScore, createdAtActionResult.Value);
            _mockRepo.Verify(repo => repo.SaveScore(newScore), Times.Once);
        }

        [Fact]
        public async Task SaveScore_ReturnsBadRequest_WhenExceptionThrown()
        {
            var newScore = new Score { Id = 1, PlayerName = "Player1", GameName = "FinderGame", Value = 100 };
            _mockRepo.Setup(repo => repo.SaveScore(newScore)).ThrowsAsync(new Exception("Error"));

            var result = await _controller.SaveScore(newScore);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error", badRequestResult.Value);
            _mockRepo.Verify(repo => repo.SaveScore(newScore), Times.Once);
        }
    }
}
