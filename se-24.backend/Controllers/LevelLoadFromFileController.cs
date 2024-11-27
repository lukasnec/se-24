using Microsoft.AspNetCore.Mvc;
using se_24.backend.src.Interfaces;
using se_24.backend.src.Repositories;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using System.Collections.Concurrent;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelLoadFromFileController : ControllerBase
    {
        private readonly ILevelFilesRepository _levelFilesRepository;
        private readonly ILevelLoader<Level> _finderLevelLoader;
        private readonly ILevelLoader<ReadingLevel> _readingLevelLoader;
        public LevelLoadFromFileController(
            ILevelFilesRepository levelFilesRepository, 
            ILevelLoader<Level> finderLevelLoader,
            ILevelLoader<ReadingLevel> readingLevelLoader
            )
        {
            _levelFilesRepository = levelFilesRepository;
            _finderLevelLoader = finderLevelLoader;
            _readingLevelLoader = readingLevelLoader;
        }

        [HttpGet("FinderGameLevels")]
        public ActionResult LoadFinderGameLevels()
        {
            string path = "Files/Levels/FinderGame/";
            try
            {
                var levels = _finderLevelLoader.LoadAllLevels(path);
                _levelFilesRepository.SaveFinderGameLevels(levels);
                return Ok($"Added {levels.Count} levels.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load levels: {ex.Message}");
                return BadRequest($"Failed to load levels: {ex.Message}");
            }
        }


        [HttpGet("ReadingGameLevels")]
        public ActionResult LoadReadingGameLevels()
        {
            string path = "Files/Levels/ReadingGame/";
            try
            {
                var levels = _readingLevelLoader.LoadAllLevels(path);
                _levelFilesRepository.SaveReadingGameLevels(levels);
                return Ok($"Added {levels.Count} ReadingGame levels.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load ReadingGame levels: {ex.Message}");
                return BadRequest($"Failed to load ReadingGame levels: {ex.Message}");
            }
        }
    }
}
