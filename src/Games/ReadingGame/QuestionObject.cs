namespace src.Games.ReadingGame
{
    public class QuestionObject
    {
        public string Question { get; set; } = "";
        public string[] Answers { get; set; } = new string[4];
        public int CorrectAnswers { get; set; } = 0;
        public int Level { get; set; } = 1;
    }
}
