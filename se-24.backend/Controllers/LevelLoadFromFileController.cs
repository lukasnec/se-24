using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.FileManipulation;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelLoadFromFileController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly LevelLoader _levelLoader = new LevelLoader();
        public LevelLoadFromFileController(IDbContextFactory<AppDbContext> DbFactory)
        {
            _dbFactory = DbFactory;
        }

        [HttpGet("FinderGameLevels")]
        public ActionResult LoadFinderGameLevels()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            string path = "Files/Levels/FinderGame/";
            List<Level> levels = _levelLoader.LoadAllLevels<Level>(path);
            try
            {
                dbContext.FinderLevels.AddRange(levels);
                dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
                return BadRequest("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
            }
            
            return Ok("Added " + levels.Count + " levels");
        }

        [HttpGet("ReadingGameLevels")]
        public ActionResult LoadReadingGameLevels()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            string path = "Files/Levels/ReadingGame/";
            List<ReadingLevel> levels = _levelLoader.LoadAllLevels<ReadingLevel>(path);
            try
            {
                dbContext.ReadingLevels.AddRange(levels);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
                return BadRequest("Failed to save levels: " + ex.Message + '\n' + ex.InnerException);
            }
            return Ok("Added " + levels.Count + " levels");
        }
    }
}
