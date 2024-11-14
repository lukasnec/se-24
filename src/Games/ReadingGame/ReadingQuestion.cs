namespace src.Games.ReadingGame
{
    public class ReadingQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = "";
        public string[] Answers { get; set; } = new string[4];
        public int CorrectAnswer { get; set; } = 0;
        public int Level { get; set; } = 1;
        public ReadingLevel? ReadingLevel { get; set; }
    }
}
