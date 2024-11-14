namespace src.Games.ReadingGame
{
    public class ReadingLevel
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public int ReadingTime { get; set; } = 60;
        public List<ReadingQuestion> Questions { get; set; } = [];
    }
}
