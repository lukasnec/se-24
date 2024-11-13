namespace src.Authentification
{
    public class UserStateService
    {
        private bool IsAuthenticated { get; set; } = false;
        private bool IsGuest { get; set; } = false;

        public void SetAuthenticated(bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
        }

        public bool GetAuthenticated()
        {
            return IsAuthenticated;
        }

        public void SetGuest(bool guest) {
            IsGuest = guest;
        }

        public bool GetGuest() {
            return IsGuest;
        }
    }
}
