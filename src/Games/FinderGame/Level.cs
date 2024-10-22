using src.Shared;

namespace src.Games.FinderGame
{
    public class Level
    {
        public int Id { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int GivenTime { get; set; }
        public List<GameObject> GameObjects { get; set; } = [];
    }
}
