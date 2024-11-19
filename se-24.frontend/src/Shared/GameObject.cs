namespace src.Shared
{
    public class GameObject
    {
        public string Name { get; set; } = "";
        public required string Image { get; set; }
        public required Position Position { get; set; } // GameObject position on screen (percentile)
        public bool IsFound { get; set; } = false;
    }
}
