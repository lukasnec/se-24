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
        private readonly ILevelLoader _levelLoader;
        public LevelLoadFromFileController(ILevelFilesRepository levelFilesRepository, ILevelLoader levelLoader)
        {
            _levelFilesRepository = levelFilesRepository;
            _levelLoader = levelLoader;
        }

        [HttpGet("FinderGameLevels")]
        public ActionResult LoadFinderGameLevels()
        {
            string path = "Files/Levels/FinderGame/";
            try
            {
                var levels = _levelLoader.LoadAllLevels<Level>(path);
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
                var levels = _levelLoader.LoadAllLevels<ReadingLevel>(path);
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
