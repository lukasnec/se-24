namespace src.Games.FinderGame
{
    public class Level
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int GivenTime { get; set; }
        public List<GameObject> GameObjects { get; set; } = [];
    }
}
