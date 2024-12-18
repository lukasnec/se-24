using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;

namespace se_24.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinderLevelsController : ControllerBase
    {
        private readonly IFinderLevelRepository _finderLevelRepository;
        public FinderLevelsController(IFinderLevelRepository finderLevelRepository) 
        {
            _finderLevelRepository = finderLevelRepository;
        }

        [HttpGet]
        public async Task<ActionResult<Level>> GetFinderGameLevels()
        {
            var levels = await _finderLevelRepository.GetFinderGameLevels();
            return Ok(levels);
        }

        [HttpGet("{difficulty}")]
        public async Task<ActionResult<Level>> GetFinderGameLevelsByDifficulty(string difficulty)
        {
            var levels = await _finderLevelRepository.GetFinderGameLevelsByDifficulty(difficulty);
            return Ok(levels);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetFinderGameLevelsCount()
        {
            var count = await _finderLevelRepository.GetFinderGameLevelsCount();
            Console.WriteLine($"Finder Levels Count: {count}"); // Log to console
            return Ok(count);
        }


    }
}
