using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.src.Repositories
{
    public class ReadingLevelRepository : IReadingLevelRepository
    {
        private readonly AppDbContext _dbContext;

        public ReadingLevelRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ReadingLevel>> GetReadingGameLevels()
        {
            return await _dbContext.ReadingLevels
                .Include(rl => rl.Questions)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReadingLevel>> GetReadingGameLevelsByReadingLevel(int readingLevel)
        {
            return await _dbContext.ReadingLevels
                .Include(rl => rl.Questions)
                .Where(level => level.Level == readingLevel)
                .ToListAsync();
        }
    }
}
