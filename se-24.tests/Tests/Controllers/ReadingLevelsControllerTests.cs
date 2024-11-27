using Microsoft.AspNetCore.Mvc;
using Moq;
using se_24.backend.Controllers;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.ReadingGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace se_24.tests.Tests.Controllers
{
    public class ReadingLevelsControllerTests
    {
        private readonly Mock<IReadingLevelRepository> _mockRepo;
        private readonly ReadingLevelsController _controller;

        public ReadingLevelsControllerTests()
        {
            _mockRepo = new Mock<IReadingLevelRepository>();
            _controller = new ReadingLevelsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetReadingGameLevels_ReturnsListOfLevels()
        {
            var mockLevels = new List<ReadingLevel>
            {
                new ReadingLevel { Level = 1},
                new ReadingLevel { Level = 2 }
            };
            _mockRepo.Setup(repo => repo.GetReadingGameLevels()).ReturnsAsync(mockLevels);

            var result = await _controller.GetReadingGameLevels();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReadingLevel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetReadingGameLevelsByReadingLevel_ReturnsLevel_WithSelectedReadingLevel()
        {
            int readingLevel = 1;
            var mockLevels = new List<ReadingLevel>
            {
                new ReadingLevel { Level = 1}
            };
            _mockRepo.Setup(repo => repo.GetReadingGameLevelsByReadingLevel(readingLevel)).ReturnsAsync(mockLevels);

            var result = await _controller.GetReadingGameLevelsByReadingLevel(readingLevel);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ReadingLevel>>(okResult.Value);
            Assert.Equal(readingLevel, returnValue[0].Level);
        }
    }
}
