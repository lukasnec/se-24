using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.shared.src.Shared;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        public ScoreController(IDbContextFactory<AppDbContext> DbFactory)
        {
            _dbFactory = DbFactory;
        }

        [HttpGet]
        public List<Score> GetScoresByGameName()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var scores = dbContext.Scores.ToList();
            return scores;
        }

        [HttpGet("by-game/{gameName}")]
        public List<Score> GetScoresByGameName(string gameName)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var scores = dbContext.Scores
                .Where(score => score.GameName == gameName)
                .ToList();
            return scores;
        }

        [HttpGet("{id}")]
        public IActionResult GetScoreById(int id)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var score = dbContext.Scores.Find(id);
            if (score == null)
            {
                return NotFound();
            }
            return Ok(score);
        }

        [HttpPost]
        public IActionResult SaveScore([FromBody] Score score)
        {
            if (score == null)
            {
                return BadRequest("Score object is null.");
            }

            if (string.IsNullOrEmpty(score.GameName) || string.IsNullOrEmpty(score.PlayerName) || score.value < 0)
            {
                return BadRequest("Invalid score data.");
            }

            using var dbContext = _dbFactory.CreateDbContext();
            dbContext.Scores.Add(score);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetScoreById), new { id = score.Id }, score);
        }
    }
}
