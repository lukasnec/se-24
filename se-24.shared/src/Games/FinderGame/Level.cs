using se_24.shared.src.Shared;
using System.ComponentModel.DataAnnotations;

namespace se_24.shared.src.Games.FinderGame
{
    public class Level : IComparable<Level>
    {
        [Key]
        public int Id { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int GivenTime { get; set; }
        public List<GameObject> GameObjects { get; set; } = [];

        public int CompareTo(Level other)
        {
            // Made this in reverse to sort descendingly
            return other.GivenTime.CompareTo(GivenTime);
        }
    }
}
