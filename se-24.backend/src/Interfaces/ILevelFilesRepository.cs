using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.src.Interfaces
{
    public interface ILevelFilesRepository
    {
        Task SaveFinderGameLevels(IEnumerable<Level> levels);
        Task SaveReadingGameLevels(IEnumerable<ReadingLevel> levels);
    }
}
