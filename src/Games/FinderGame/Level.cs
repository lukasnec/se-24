using src.Shared;

namespace src.Games.FinderGame
{
    public class Level : IComparable<Level>
    {
        public int Id { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int GivenTime { get; set; }
        public List<GameObject> GameObjects { get; set; } = [];

        public int CompareTo(Level other)
        {
            // Made this in reverse to sort descendingly
            return other.GivenTime.CompareTo(this.GivenTime);
        }
    }
}
