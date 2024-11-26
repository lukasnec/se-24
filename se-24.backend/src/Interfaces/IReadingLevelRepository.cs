using se_24.shared.src.Games.ReadingGame;

namespace se_24.backend.src.Interfaces
{
    public interface IReadingLevelRepository
    {
        Task<IEnumerable<ReadingLevel>> GetReadingGameLevels();
        Task<IEnumerable<ReadingLevel>> GetReadingGameLevelsByReadingLevel(int readingLevel);
    }
}
