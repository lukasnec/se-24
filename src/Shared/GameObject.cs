namespace src.Shared
{
    public class GameObject
    {
        public string Name { get; set; } = "";
        public string Image { get; set; }
        public int PositionX { get; set; } // x of position (percentile)
        public int PositionY { get; set; } // y of position (percentile)
        public bool IsFound { get; set; } = false;
    }
}
