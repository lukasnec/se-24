using Microsoft.AspNetCore.Components;
using se_24.shared.src.Games.FinderGame;
using src.Enums;
using se_24.shared.src.Shared;
using System.Text.Json;
using se_24.shared.src.Exceptions;

namespace se_24.frontend.Components.Pages;

public partial class FinderGame
{
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILogger<FinderGame> Logger {  get; set; }
    [Inject] private HttpClient HttpClient { get; set; }
    private readonly UsernameGenerator _usernameGenerator = new UsernameGenerator();

    private bool isLoading = false;
    private bool completedLevels = false;
    public int score = 0;
    public string username = string.Empty;

    private bool errorHappend = false;
    private string errorMessage = string.Empty;
    public string selectedDifficulty = string.Empty;
    private GameState gameState = GameState.Waiting;
    public int objectsFound = 0;

    private List<Level> levels = [];
    private List<Level> currentLevels = [];
    private int currentLevelIndex;

    private static System.Timers.Timer timer;
    public int defaultTime = 20; // Time given for a level
    private int counter = 10; // Time shown to user
    public int totalElapsedTime = 0;
    public int totalGivenTime = 0;

    protected override void OnInitialized()
    {
        currentLevelIndex = 0;
    }

    public async Task<List<Level>> GetGameLevels(string difficulty)
    {
        string url = $"FinderLevels/{difficulty}";

        try
        {
            HttpResponseMessage response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<Level>>(jsonResponse, options);
        }
        catch (Exception ex)
        {
            throw new ApiException(ex.Message);
        }
    }

    public async Task SetDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        try
        {
            currentLevels = await GetGameLevels(selectedDifficulty);
            currentLevels.Sort();
            isLoading = true;
            if (currentLevels.Count > 0)
            {
                defaultTime = currentLevels[currentLevelIndex].GivenTime;
            }
            Console.WriteLine(currentLevels.Count);
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex.Message);
            errorMessage = "Failed loading levels! Try again later.";
            errorHappend = true;
        }
        finally
        {
            isLoading = false;
        }
    }

    public void StartGame()
    {
        gameState = GameState.Started;
        counter = defaultTime;
        totalGivenTime += defaultTime;
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += CountDownTimer;
        timer.Enabled = true;
    }

    public Level GetCurrentLevel()
    {
        return currentLevels[currentLevelIndex];
    }

    public void CountDownTimer(object source, System.Timers.ElapsedEventArgs e)
    {
        if (counter > 0 && !CheckIfAllObjectsFound())
        {
            counter--;
            totalElapsedTime++;
        }
        else
        {
            if (counter == 0 && !CheckIfAllObjectsFound())
            {
                gameState = GameState.Failed;
            }
            timer.Enabled = false;
            timer.Dispose();
        }
        InvokeAsync(StateHasChanged);
    }

    public void ObjectClicked(GameObject obj)
    {
        if (!obj.IsFound && counter > 0)
        {
            obj.IsFound = true;
            objectsFound++;
        }
    }

    public void ReturnToHomePage()
    {
        NavigationManager.NavigateTo("/");
    }

    public void ReloadLevel()
    {
        foreach (var obj in currentLevels[currentLevelIndex].GameObjects)
        {
            obj.IsFound = false;
        }
        gameState = GameState.Waiting;
        counter = defaultTime;
        objectsFound = 0;
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        defaultTime = currentLevels[currentLevelIndex].GivenTime;
        gameState = GameState.Waiting;
        objectsFound = 0;   
    }

    public bool CheckIfAllObjectsFound()
    {
        if (currentLevelIndex == currentLevels.Count - 1)
        {
            completedLevels = true;
        }
        return objectsFound == currentLevels[currentLevelIndex].GameObjects.Count;
    }

    public GameState GetCurrentGameState()
    {
        return gameState;
    }

    public void CalculateScore()
    {
        int difficultyMultiplier = 1;
        switch(selectedDifficulty.ToLower())
        {
            case "easy":
                difficultyMultiplier = 1;
                break;
            case "medium":
                difficultyMultiplier = 2;
                break;
            case "hard":
                difficultyMultiplier = 3;
                break;
        }
        score = difficultyMultiplier * (totalGivenTime - totalElapsedTime);
    }

    public async Task SaveScore()
    {
        if(string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            username = _usernameGenerator.GenerateGuestName();
        }
        Score score = new Score
        {
            PlayerName = username,
            GameName = "FinderGame",
            value = this.score
        };

        string url = "score";

        try
        {
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(url, score);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Score successfully posted!");
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseBody);
            }
            else
            {
                Console.WriteLine($"Failed to post score. Status Code: {response.StatusCode}");
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while posting the score: " + ex.Message);
        }
    }
}
