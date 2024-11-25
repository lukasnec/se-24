using Microsoft.AspNetCore.Mvc;
using se_24.shared.src.Shared;
using System.Collections;

namespace se_24.backend.src.Interfaces
{
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetScores();
        Task<IEnumerable<Score>> GetScoresByGameName(string gameName);
        Task<Score> GetScoreById(int id);
        Task SaveScore(Score score);
    }
}
