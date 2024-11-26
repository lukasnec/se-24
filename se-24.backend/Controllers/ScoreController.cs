using Microsoft.AspNetCore.Mvc;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Shared;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IScoreRepository _scoreRepository;
        public ScoreController(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Score>> GetScores()
        {
            var scores = await _scoreRepository.GetScores();
            return Ok(scores);
        }

        [HttpGet("by-game/{gameName}")]
        public async Task<ActionResult<Score>> GetScoresByGameName(string gameName)
        {
            var scores = await _scoreRepository.GetScoresByGameName(gameName);
            return Ok(scores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScoreById(int id)
        {
            try
            {
                var score = await _scoreRepository.GetScoreById(id);
                return Ok(score);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveScore(Score score)
        {
            try
            {
                await _scoreRepository.SaveScore(score);
                return CreatedAtAction(nameof(GetScoreById), new { id = score.Id }, score);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
