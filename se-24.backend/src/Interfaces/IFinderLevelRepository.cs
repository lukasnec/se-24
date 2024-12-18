using se_24.shared.src.Games.FinderGame;

namespace se_24.backend.src.Interfaces
{
    public interface IFinderLevelRepository
    {
        Task<List<Level>> GetFinderGameLevels();
        Task<List<Level>> GetFinderGameLevelsByDifficulty(string difficulty);
        Task<int> GetFinderGameLevelsCount();
    }
}
