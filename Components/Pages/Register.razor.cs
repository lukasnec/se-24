using Microsoft.AspNetCore.Components;
using src.Authentification;

namespace Components.Pages
{
    public partial class Register : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        private UserStateService UserState { get; set; }
        private string Username { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        private string RepeatPassword { get; set; } = string.Empty;

        public string errorMessage = string.Empty;

        private async void RegisterUser()
        {
            if (Password != RepeatPassword)
            {
                errorMessage = "Passwords do not match!";
                return;
            }
            else if(Username == string.Empty || Password == string.Empty || RepeatPassword == string.Empty)
            {
                errorMessage = "Please fill in all the fields!";
                return;
            }
            else
            {
                //SOMEKIND OF DB INTEGRATION
                //SAVE USER DATA
                
                Navigation.NavigateTo("/login");
            }
        }

        private void AssignGuest()
        {
            UserState.SetGuest(true);
            Navigation.NavigateTo("/games");
        }
    }
}
