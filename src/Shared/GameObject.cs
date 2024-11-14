using src.Games.FinderGame;

namespace src.Shared
{
    public class GameObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public required string Image { get; set; }
        public required Position Position { get; set; } // GameObject position on screen (percentile)
        public bool IsFound { get; set; } = false;
        public Level? Level { get; set; }
    }
}
