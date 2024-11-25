using Microsoft.EntityFrameworkCore;
using se_24.backend.src.Data;
using se_24.backend.src.Interfaces;
using se_24.shared.src.Shared;

namespace se_24.backend.src.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _dbContext;

        public ScoreRepository(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Score>> GetScores()
        {
            return await _dbContext.Scores.ToListAsync();
        }

        public async Task<IEnumerable<Score>> GetScoresByGameName(string gameName)
        {
            return await _dbContext.Scores
                        .Where(score => score.GameName == gameName)
                        .ToListAsync();
        }

        public async Task<Score> GetScoreById(int id)
        {
            var score = await _dbContext.Scores.FindAsync(id);
            if (score == null)
            {
                throw new Exception("Score of the given ID not found");
            }
            return score;
        }

        public async Task SaveScore(Score score)
        {
            if (score == null)
            {
                throw new Exception("Score object is null.");
            }

            if (string.IsNullOrEmpty(score.GameName) || string.IsNullOrEmpty(score.PlayerName) || score.value < 0)
            {
                throw new Exception("Invalid score data.");
            }

            await _dbContext.Scores.AddAsync(score);
            await _dbContext.SaveChangesAsync();
        }
    }
}
