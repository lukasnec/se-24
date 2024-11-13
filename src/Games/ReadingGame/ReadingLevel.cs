namespace src.Games.ReadingGame
{
    public class ReadingLevel
    {
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public int ReadingTime { get; set; } = 60;
        public ReadingQuestion[] Questions { get; set; } = new ReadingQuestion[0];
    }
}
