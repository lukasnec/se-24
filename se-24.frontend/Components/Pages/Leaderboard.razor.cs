using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using se_24.shared.src.Shared;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace se_24.frontend.Components.Pages
{
    public partial class Leaderboard
    {
        public List<Score> Scores { get; set; } = new();
        public string selectedGameName = "All";
        public int topRanks = 0;

        protected override async Task OnInitializedAsync()
        {

            Scores = await GetScoresAsync();
            await InvokeAsync(StateHasChanged);
        }

        public async Task<List<Score>> GetScoresAsync()
        {
            string url = $"https://localhost:7077/api/Score";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<Score>>(jsonResponse, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching scores: {ex.Message}");
                throw;
            }
        }

        private IEnumerable<Score> FilteredScores => Scores
        .Where(s => selectedGameName == "All" || s.GameName == selectedGameName)
        .OrderByDescending(s => s.value)
        .Take(topRanks > 0 ? topRanks : Scores.Count);

    }
}
