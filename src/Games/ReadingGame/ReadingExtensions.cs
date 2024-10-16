namespace src.Games.ReadingGame
{
    public static class ReadingExtensions
{
        public static double GetCorrectPercentage(this QuestionClass[] questions, int correctAnswers)
        {
            if (questions.Length == 0) return 0;
            return (correctAnswers / (double)questions.Length) * 100;
        }
    }
}
