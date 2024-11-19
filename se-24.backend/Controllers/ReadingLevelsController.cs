using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingLevelsController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        public ReadingLevelsController(IDbContextFactory<AppDbContext> DbFactory)
        {
            _dbFactory = DbFactory;
        }

        [HttpGet("{readingLevel}")]
        public List<ReadingLevel> GetReadingGameLevels(int readingLevel)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var levels = dbContext.ReadingLevels
                .Include(rl => rl.Questions)
                .Where(level => level.Level == readingLevel)
                .ToList();
            return levels;
        }
    }
}
