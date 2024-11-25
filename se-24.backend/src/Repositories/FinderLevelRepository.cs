using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;

namespace se_24.backend.src.Repositories
{
    public class FinderLevelRepository : IFinderLevelRepository
    {
        private readonly AppDbContext _dbContext;

        public FinderLevelRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Level>> GetFinderGameLevels()
        {
            return await _dbContext.FinderLevels
                .Include(l => l.GameObjects)
                .ToListAsync();
        }

        public async Task<List<Level>> GetFinderGameLevelsByDifficulty(string difficulty)
        {
            return await _dbContext.FinderLevels
                .Include(l => l.GameObjects)
                .Where(level => level.Difficulty.ToLower() == difficulty.ToLower())
                .ToListAsync();
        }
    }
}
