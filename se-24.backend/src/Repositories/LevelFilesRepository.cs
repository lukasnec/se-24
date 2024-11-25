using se_24.backend.src.Data;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.src.Repositories
{
    public class LevelFilesRepository : ILevelFilesRepository
    {
        private readonly AppDbContext _dbContext;
        public LevelFilesRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveFinderGameLevels(IEnumerable<Level> levels)
        {
            try
            {
                await _dbContext.FinderLevels.AddRangeAsync(levels);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task SaveReadingGameLevels(IEnumerable<ReadingLevel> levels)
        {
            try
            {
                await _dbContext.ReadingLevels.AddRangeAsync(levels);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
