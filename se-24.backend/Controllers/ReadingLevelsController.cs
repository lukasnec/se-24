using Microsoft.AspNetCore.Mvc;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingLevelsController : ControllerBase
    {
        private readonly IReadingLevelRepository _readingLevelRepository;
        public ReadingLevelsController(IReadingLevelRepository readingLevelRepository)
        {
            _readingLevelRepository = readingLevelRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ReadingLevel>> GetReadingGameLevels()
        {
            var levels = await _readingLevelRepository.GetReadingGameLevels();
            return Ok(levels);
        }

        [HttpGet("{readingLevel}")]
        public async Task<ActionResult<ReadingLevel>> GetReadingGameLevelsByReadingLevel(int readingLevel)
        {
            var levels = await _readingLevelRepository.GetReadingGameLevelsByReadingLevel(readingLevel);
            return Ok(levels);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetReadingGameLevelsCount()
        {
            var count = await _readingLevelRepository.GetReadingGameLevelsCount();
            return Ok(count);
        }

    }
}
