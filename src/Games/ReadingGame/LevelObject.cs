namespace se_24.src.Games.ReadingGame
{
    public class LevelObject
    {
        public int level { get; set; }
        public string text { get; set; } = "";
        public int readingTime { get; set; } = 60;
        public QuestionObject[] questions { get; set; } = new QuestionObject[0];
    }
}
