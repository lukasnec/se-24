using System.ComponentModel.DataAnnotations;

namespace se_24.shared.src.Games.ReadingGame
{
    public class ReadingLevel
    {
        [Key]
        public int Id;
        public int Level { get; set; }
        public string Text { get; set; } = "";
        public int ReadingTime { get; set; } = 60;
        public List<ReadingQuestion> Questions { get; set; } = [];
    }
}
