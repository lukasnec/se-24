using System.Net.Http.Json;

namespace se_24.frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password)
        {
            var user = new { Username = username, Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("User/register", user);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> LoginUserAsync(string username, string password)
        {
            var user = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("User/login", user);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token ?? string.Empty;
            }

            throw new Exception(await response.Content.ReadAsStringAsync());
        }

        private class LoginResponse
        {
            public string Token { get; set; }
        }
    }
}
