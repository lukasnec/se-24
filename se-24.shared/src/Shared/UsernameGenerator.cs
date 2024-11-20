namespace se_24.shared.src.Shared
{
    public class UsernameGenerator
    {
        public string GenerateGuestName()
        {
            Random rnd = new Random();
            int number = rnd.Next(100000, 1000000); // Generates a random 6 digit number
            return "guest-" + number;
        }
    }
}
