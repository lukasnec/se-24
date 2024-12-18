using Microsoft.AspNetCore.Components;
using se_24.shared.src.Shared;
using System.Text.Json;

namespace se_24.frontend.Components.Pages
{
    public partial class Leaderboard
    {
        [Inject] private HttpClient HttpClient { get; set; }
        public List<Score> Scores { get; set; } = new();
        public string selectedGameName = "All";
        public int topRanks = 0;
        public string errorMessage = string.Empty;

        public async Task OnInitializedAsync()
        {
            try
            {
                Scores = await GetScoresAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (HttpRequestException)
            {
                errorMessage = "Failed to load leaderboard.";
            }
        }

        public async Task<List<Score>> GetScoresAsync()
        {
            string url = $"Score";

            try
            {
                HttpResponseMessage response = await HttpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Score>>(jsonResponse, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching scores: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Score> FilteredScores => Scores
        .Where(s => selectedGameName == "All" || s.GameName == selectedGameName)
        .OrderByDescending(s => s.Value)
        .Take(topRanks > 0 ? topRanks : Scores.Count);

    }
}
