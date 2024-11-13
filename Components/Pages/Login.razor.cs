using Microsoft.AspNetCore.Components;
using src.Authentification;

namespace Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserStateService UserState { get; set; }
        private string Username { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        public string errorMessage = string.Empty;

        private async void LoginUser()
        {
            if (Username == string.Empty || Password == string.Empty)
            {
                errorMessage = "Please fill in all the fiels";
                return;
            }
            else if (Username == "Admin" && Password == "Admin")
            {
                //SOMEKIND OF DB INTEGRATION
                //CHECK USER DATA
                Console.WriteLine("Login successful");
                UserState.SetAuthenticated(true);
                Navigation.NavigateTo("/games");
            }
        }
        private void AssignGuest()
        {
            UserState.SetGuest(true);
            Navigation.NavigateTo("/games");
        }
    }
}
