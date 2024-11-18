using se_24.shared.src.Games.FinderGame;
using System.ComponentModel.DataAnnotations;

namespace se_24.shared.src.Shared
{
    public class GameObject
    {
        [Key]
        public int Id;
        public string Name { get; set; } = "";
        public required string Image { get; set; }
        public required Position Position { get; set; } // GameObject position on screen (percentile)
        public bool IsFound { get; set; } = false;
    }
}
