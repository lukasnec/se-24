using Microsoft.AspNetCore.Components;
using se_24.shared.src.Shared;
using src.Enums;
using src.Games.BlockGame;

namespace se_24.frontend.Components.Pages
{

    public partial class BlockGame : ComponentBase
    {
        // Static Random instance for randomness
        private static readonly Random random = new Random();

        [Inject] private HttpClient HttpClient { get; set; }
        public List<GameMove> Sequence { get; set; } = new List<GameMove>();

        public PlayerStats playerStats = new PlayerStats(0);
        private readonly UsernameGenerator _usernameGenerator = new UsernameGenerator();
        public int CurrentStep { get; set; } = 0;
        public string statusMessage = "Press 'Start New Round' to begin.";
        private bool isAnimatingSequence = false;
        public bool showFinalScore = false;
        private int? activeSquare = null;
        private int? clickedSquare = null;
        public int score = 0;
        public string username = string.Empty;

        public GameState CurrentGameState { get; set; } = GameState.Waiting;

        public int roundNumber = 1;

        [Inject] private NavigationManager Navigation { get; set; }

        public async Task StartGame()
        {
            CurrentGameState = GameState.Started;
            playerStats = new PlayerStats(0);  // Reset the player's correct sequence count using 'record'
            roundNumber = 1;
            await StartNewRound();
        }

        public async Task OnPlayerClick(int squareId)
        {
            if (isAnimatingSequence || CurrentGameState != GameState.Started) return;

            clickedSquare = squareId;
            await InvokeAsync(StateHasChanged);

            await Task.Delay(500);
            clickedSquare = null;
            await InvokeAsync(StateHasChanged);

            if (CheckPlayerInput(squareId))
            {
                CurrentStep++;
                if (IsRoundComplete())
                {
                    statusMessage = "Correct! Starting next round...";
                    playerStats = playerStats with { CorrectSequenceCount = playerStats.CorrectSequenceCount + 1 };
                    roundNumber++;
                    CurrentGameState = GameState.Finished;
                    await Task.Delay(1000);
                    await StartNewRound();
                }
            }
            else
            {
                statusMessage = "Wrong! Game over. Press 'Start New Round' to try again.";
                CurrentGameState = GameState.Failed;
                showFinalScore = true;
                ResetGame();
            }
            await InvokeAsync(StateHasChanged);
        }

        public async Task StartNewRound()
        {
            showFinalScore = false;
            isAnimatingSequence = true;
            statusMessage = "Watch the sequence...";
            CurrentGameState = GameState.Started;

            GenerateNewRandomSequence(roundNumber);

            foreach (var move in Sequence)
            {
                activeSquare = move.SquareId;
                await InvokeAsync(StateHasChanged);
                await Task.Delay(800); // Delay to show square lit
                activeSquare = null;
                await InvokeAsync(StateHasChanged);
                await Task.Delay(300); // Short delay between squares
            }

            statusMessage = "Your turn! Repeat the sequence.";
            await InvokeAsync(StateHasChanged);
            isAnimatingSequence = false;
            CurrentStep = 0;
        }

        // Generate a new random sequence of squares for this round
        public void GenerateNewRandomSequence(int sequenceLength)
        {
            Sequence.Clear();
            for (int i = 0; i < sequenceLength; i++)
            {
                int newSquareId = random.Next(1, 5);
                Sequence.Add(new GameMove(newSquareId)); // Add the new square to the sequence using 'record'
            }
        }

        public bool CheckPlayerInput(int input)
        {
            if (CurrentStep >= Sequence.Count || CurrentStep < 0) return false;
            return Sequence[CurrentStep].SquareId == input;
        }

        private bool IsRoundComplete() => CurrentStep >= Sequence.Count;

        public void EndGame()
        {
            statusMessage = "Game ended by the player.";
            CurrentGameState = GameState.Finished;
        }

        private void ResetGame()
        {
            Sequence.Clear();
            roundNumber = 1; // Reset the round number when the game is reset
            CurrentStep = 0;
            isAnimatingSequence = false;
            CurrentGameState = GameState.Waiting;
        }

        private bool IsSquareLit(int squareId) => activeSquare == squareId;

        private bool IsSquareClicked(int squareId) => clickedSquare == squareId;

        private bool DisableSquareClick() => CurrentGameState != GameState.Started;

        public void CalculateScore()
        {
            score = playerStats.CorrectSequenceCount;
        }

        public async Task SaveScore()
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
            {
                username = _usernameGenerator.GenerateGuestName();
            }
            Score score = new Score
            {
                PlayerName = username,
                GameName = "BlockGame",
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
}
