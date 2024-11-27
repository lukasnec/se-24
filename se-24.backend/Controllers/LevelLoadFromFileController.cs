using Microsoft.AspNetCore.Mvc;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

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
                return Ok("Added " + levels.Count + " levels");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
                return BadRequest("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
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
                return Ok("Added " + levels.Count + " levels");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
                return BadRequest("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
            }
        }
    }
}
