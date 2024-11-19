using Microsoft.AspNetCore.Components;
using se_24.shared.src.Games.FinderGame;
using src.Enums;
using se_24.shared.src.Shared;
using System.Text.Json;

namespace se_24.frontend.Components.Pages;

public partial class FinderGame
{
    [Inject] private NavigationManager NavigationManager { get; set; }

    private bool isLoading = false;

    private string selectedDifficulty = string.Empty;
    private GameState gameState = GameState.Waiting;
    private int objectsFound = 0;

    private List<Level> levels = [];
    private List<Level> currentLevels = [];
    private int currentLevelIndex;

    private static System.Timers.Timer timer;
    private int defaultTime = 20; // Time given for a level
    private int counter = 10; // Time shown to user

    protected override void OnInitialized()
    {
        currentLevelIndex = 0;
    }

    public async Task<List<Level>> GetGameLevels(string difficulty)
    {
        string url = $"https://localhost:7077/api/FinderLevels/{difficulty}";

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
                return JsonSerializer.Deserialize<List<Level>>(jsonResponse, options);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching levels: {ex.Message}");
            throw;
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting difficulty: {ex.Message}");
            throw;
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
            counter -= 1;
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
        if (currentLevelIndex == currentLevels.Count - 1)
        {
            currentLevelIndex = 0;
            foreach (Level level in currentLevels)
            {
                foreach (GameObject obj in level.GameObjects)
                {
                    obj.IsFound = false;
                }
            }
        }
        else
        {
            currentLevelIndex++;
        }
        defaultTime = currentLevels[currentLevelIndex].GivenTime;
        gameState = GameState.Waiting;
        objectsFound = 0;
    }

    public bool CheckIfAllObjectsFound()
    {
        return objectsFound == currentLevels[currentLevelIndex].GameObjects.Count;
    }

    public GameState GetCurrentGameState()
    {
        return gameState;
    }
}
