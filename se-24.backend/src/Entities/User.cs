using System.ComponentModel.DataAnnotations;

namespace se_24.backend.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; } // Hashed password
        public string Role { get; set; } = "User"; // Default role
    }
}
