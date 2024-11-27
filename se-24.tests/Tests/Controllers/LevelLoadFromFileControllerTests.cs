using Microsoft.AspNetCore.Mvc;
using Moq;
using se_24.backend.Controllers;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.tests.Tests.Controllers
{
    public class LevelLoadFromFileControllerTests
    {
        private readonly Mock<ILevelFilesRepository> _mockRepo;
        private readonly Mock<ILevelLoader<Level>> _mockFinderLevelLoader;
        private readonly Mock<ILevelLoader<ReadingLevel>> _mockReadingLevelLoader;
        private readonly LevelLoadFromFileController _controller;

        public LevelLoadFromFileControllerTests()
        {
            _mockRepo = new Mock<ILevelFilesRepository>();
            _mockFinderLevelLoader = new Mock<ILevelLoader<Level>>();
            _mockReadingLevelLoader = new Mock<ILevelLoader<ReadingLevel>>();
            _controller = new LevelLoadFromFileController(_mockRepo.Object, _mockFinderLevelLoader.Object, _mockReadingLevelLoader.Object);
        }

        [Fact]
        public void LoadFinderGameLevels_Success_ReturnsOkResult()
        {
            var mockLevels = new List<Level>
            {
                new Level { Id = 1, GivenTime = 60 },
                new Level { Id = 2, GivenTime = 60 }
            };
            _mockFinderLevelLoader.Setup(loader => loader.LoadAllLevels("Files/Levels/FinderGame/"))
                .Returns(mockLevels);
            _mockRepo.Setup(repo => repo.SaveFinderGameLevels(mockLevels));

            var result = _controller.LoadFinderGameLevels();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Added 2 levels.", okResult.Value);
        }

        [Fact]
        public void LoadFinderGameLevels_ExceptionThrown_ReturnsBadRequest()
        {
            _mockFinderLevelLoader.Setup(loader => loader.LoadAllLevels("Files/Levels/FinderGame/"))
                .Throws(new Exception("Load error"));

            var result = _controller.LoadFinderGameLevels();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Failed to load levels: Load error", badRequestResult.Value.ToString());
        }

        [Fact]
        public void LoadReadingGameLevels_Success_ReturnsOkResult()
        {
            var mockLevels = new List<ReadingLevel>
            {
                new ReadingLevel { Level = 1 },
                new ReadingLevel { Level = 2 }
            };
            _mockReadingLevelLoader.Setup(loader => loader.LoadAllLevels("Files/Levels/ReadingGame/"))
                .Returns(mockLevels);
            _mockRepo.Setup(repo => repo.SaveReadingGameLevels(mockLevels));

            var result = _controller.LoadReadingGameLevels();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Added 2 ReadingGame levels.", okResult.Value);
        }

        [Fact]
        public void LoadReadingGameLevels_ExceptionThrown_ReturnsBadRequest()
        {
            _mockReadingLevelLoader.Setup(loader => loader.LoadAllLevels("Files/Levels/ReadingGame/"))
                .Throws(new Exception("Load error"));

            var result = _controller.LoadReadingGameLevels();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Failed to load ReadingGame levels: Load error", badRequestResult.Value.ToString());
        }
    }
}
