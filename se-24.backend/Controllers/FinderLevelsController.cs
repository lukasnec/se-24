using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.shared.src.Games.FinderGame;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinderLevelsController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        public FinderLevelsController(IDbContextFactory<AppDbContext> DbFactory) 
        {
            _dbFactory = DbFactory;
        }

        [HttpGet("{difficulty}")]
        public List<Level> GetFinderGameLevels(string difficulty)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var levels = dbContext.FinderLevels
                .Include(l => l.GameObjects)
                .Where(level => level.Difficulty.ToLower() == difficulty.ToLower())
                .ToList();
            return levels;
        }
    }
}
