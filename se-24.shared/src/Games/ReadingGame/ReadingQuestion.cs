using System.ComponentModel.DataAnnotations;

namespace se_24.shared.src.Games.ReadingGame
{
    public class ReadingQuestion
    {
        [Key]
        public int Id;
        public string Question { get; set; } = "";
        public string[] Answers { get; set; } = new string[4];
        public int CorrectAnswer { get; set; } = 0;
        public int Level { get; set; } = 1;
    }
}
