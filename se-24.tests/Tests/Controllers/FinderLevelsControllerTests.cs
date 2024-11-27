using Microsoft.AspNetCore.Mvc;
using Moq;
using se_24.backend.Controllers;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;

namespace se_24.tests.Tests.Controllers
{
    public class FinderLevelsControllerTests
    {
        private readonly Mock<IFinderLevelRepository> _mockRepo;
        private readonly FinderLevelsController _controller;

        public FinderLevelsControllerTests()
        {
            _mockRepo = new Mock<IFinderLevelRepository>();
            _controller = new FinderLevelsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetFinderGameLevels_ReturnsListOfLevels()
        {
            var mockLevels = new List<Level>
            {
                new Level { Id = 1, GivenTime = 60 },
                new Level { Id = 2, GivenTime = 60}
            };
            _mockRepo.Setup(repo => repo.GetFinderGameLevels()).ReturnsAsync(mockLevels);

            var result = await _controller.GetFinderGameLevels();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLevels = Assert.IsType<List<Level>>(okResult.Value);
            Assert.Equal(2, returnedLevels.Count);
            _mockRepo.Verify(repo => repo.GetFinderGameLevels(), Times.Once);
        }

        [Fact]
        public async Task GetFinderGameLevelsByDifficulty_ReturnsLevels_WithSelectedDifficulty()
        {
            var difficulty = "easy";
            var mockLevels = new List<Level>
            {
                new Level { Id = 1, Difficulty = difficulty, GivenTime = 60 }
            };
            _mockRepo.Setup(repo => repo.GetFinderGameLevelsByDifficulty(difficulty)).ReturnsAsync(mockLevels);

            var result = await _controller.GetFinderGameLevelsByDifficulty(difficulty);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLevels = Assert.IsType<List<Level>>(okResult.Value);
            Assert.Single(returnedLevels);
            Assert.Equal(difficulty, returnedLevels[0].Difficulty);
            _mockRepo.Verify(repo => repo.GetFinderGameLevelsByDifficulty(difficulty), Times.Once);
        }
    }
}
