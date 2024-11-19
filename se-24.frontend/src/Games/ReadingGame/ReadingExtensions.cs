using se_24.shared.src.Games.ReadingGame;

namespace src.Games.ReadingGame
{
    public static class ReadingExtensions
{
        public static double GetCorrectPercentage(this List<ReadingQuestion> questions, int correctAnswers)
        {
            if (questions.Count == 0) return 0;
            return (correctAnswers / (double)questions.Count) * 100;
        }
    }
}
