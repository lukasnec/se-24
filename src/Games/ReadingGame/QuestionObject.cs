namespace se_24.src.Games.ReadingGame
{
    public class QuestionObject
    {
        public string question { get; set; } = "";
        public string[] answers { get; set; } = new string[4];
        public int correctAnswer { get; set; } = 0;
        public int level { get; set; } = 1;
    }
}
