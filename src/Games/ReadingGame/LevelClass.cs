namespace src.Games.ReadingGame
{
    public class LevelClass
    {
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public int ReadingTime { get; set; } = 60;
        public QuestionClass[] Questions { get; set; } = new QuestionClass[0];
    }
}
