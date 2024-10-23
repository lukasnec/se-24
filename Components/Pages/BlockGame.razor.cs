using Microsoft.AspNetCore.Components;
using src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace se_24.Components.Pages
{
    public class GameMove
    {
        public int SquareId { get; set; }

        public GameMove(int squareId)
        {
            SquareId = squareId;
        }
    }

    public class PlayerStats
    {
        public int CorrectSequenceCount { get; set; }

        public PlayerStats(int correctSequenceCount)
        {
            CorrectSequenceCount = correctSequenceCount;
        }
    }

    public partial class BlockGame : ComponentBase
    {
        // Static Random instance for randomness
        private static readonly Random random = new Random();

        // Current sequence of squares for this round
        private List<GameMove> Sequence { get; set; } = new List<GameMove>();

        private PlayerStats playerStats = new PlayerStats(0);
        private int CurrentStep { get; set; } = 0;
        private string statusMessage = "Press 'Start New Round' to begin.";
        private bool isAnimatingSequence = false;
        private int? activeSquare = null;
        private int? clickedSquare = null;

        private GameState CurrentGameState { get; set; } = GameState.Waiting;

        private int roundNumber = 1; // Track the current round number

        [Inject] private NavigationManager Navigation { get; set; }

        public void StartGame()
        {
            CurrentGameState = GameState.Started;
            playerStats = new PlayerStats(0);  // Reset the player's correct sequence count
            roundNumber = 1; // Start with the first round
            StartNewRound();
        }

        // When player clicks a square
        private async void OnPlayerClick(int squareId)
        {
            if (isAnimatingSequence || CurrentGameState != GameState.Started) return;

            clickedSquare = squareId;
            StateHasChanged();

            await Task.Delay(500);
            clickedSquare = null;
            StateHasChanged();

            if (CheckPlayerInput(squareId))
            {
                CurrentStep++;
                if (IsRoundComplete())
                {
                    statusMessage = "Correct! Starting next round...";
                    playerStats.CorrectSequenceCount++;  // Increment correct sequence count
                    roundNumber++;  // Increment round number for the next round
                    CurrentGameState = GameState.Finished;
                    await Task.Delay(1000);
                    StartNewRound();
                }
            }
            else
            {
                statusMessage = "Wrong! Game over. Press 'Start New Round' to try again.";
                CurrentGameState = GameState.Failed;
                ResetGame();
            }
        }

        // Generate a new random sequence for this round and animate it
        private async void StartNewRound()
        {
            isAnimatingSequence = true;
            statusMessage = "Watch the sequence...";
            CurrentGameState = GameState.Started;

            GenerateNewRandomSequence(roundNumber); // Generate a new sequence based on the round number

            // Play the sequence by lighting up the squares one by one
            foreach (var move in Sequence)
            {
                activeSquare = move.SquareId;
                StateHasChanged();
                await Task.Delay(800); // Delay to show square lit
                activeSquare = null;
                StateHasChanged();
                await Task.Delay(300); // Short delay between squares
            }

            statusMessage = "Your turn! Repeat the sequence.";
            isAnimatingSequence = false;
            CurrentStep = 0;
        }

        // Generate a new random sequence of squares for this round
        private void GenerateNewRandomSequence(int sequenceLength)
        {
            Sequence.Clear(); // Clear the previous sequence
            for (int i = 0; i < sequenceLength; i++)
            {
                int newSquareId = random.Next(1, 5); // Random number between 1 and 4
                Sequence.Add(new GameMove(newSquareId)); // Add the new square to the sequence
            }
        }

        private bool CheckPlayerInput(int input)
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
    }
}
