namespace src.Games
{
    public class GameObject
    {
        public string name { get; set; } = "";
        public string image { get; set; }
        public int positionX { get; set; } // x of position (percentile)
        public int positionY { get; set; } // y of position (percentile)
        public bool isFound { get; set; } = false;
    }
}
