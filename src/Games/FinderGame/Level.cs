namespace src.Games.FinderGame
{
    public class Level
    {
        public int id { get; set; }
        public string image { get; set; }

        public int givenTime { get; set; }

        public List<GameObject> gameObjects { get; set; } = [];
    }
}
