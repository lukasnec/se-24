namespace src.Games.ReadingGame
{
    public class LevelObject
    {
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public int ReadingTime { get; set; } = 60;
        public QuestionObject[] Questions { get; set; } = new QuestionObject[0];
    }
}
