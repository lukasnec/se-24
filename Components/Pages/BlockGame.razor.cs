using Microsoft.AspNetCore.Components;
using src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace se_24.Components.Pages
{
    public partial class BlockGame : ComponentBase
    {
        private List<int> Sequence { get; set; } = new List<int>();
        private int CurrentStep { get; set; } = 0;
        private string statusMessage = "Press 'Start New Round' to begin.";
        private bool isAnimatingSequence = false;
        private int? activeSquare = null;
        private int? clickedSquare = null;

        // GameState to track the current state of the game
        private GameState CurrentGameState { get; set; } = GameState.Waiting;

        // Counter to track successful sequences
        private int correctSequenceCount = 0;

        [Inject] private NavigationManager Navigation { get; set; }

        // Timer field for managing game timing
        private System.Timers.Timer? timer;

        public void StartGame()
        {
            CurrentGameState = GameState.Started;
            correctSequenceCount = 0;  // Reset the counter when starting a new game
            StartNewRound();
        }

        // Player clicks a square
        private async void OnPlayerClick(int squareId)
        {
            if (isAnimatingSequence || CurrentGameState != GameState.Started) return;

            clickedSquare = squareId;
            StateHasChanged(); // Refresh the UI to show the clicked square

            await Task.Delay(500);
            clickedSquare = null;
            StateHasChanged();

            if (CheckPlayerInput(squareId)) // Check if player's input is correct
            {
                CurrentStep++;
                if (IsRoundComplete()) // If player completes the sequence
                {
                    statusMessage = "Correct! Starting next round...";
                    correctSequenceCount++; // Increment the counter
                    CurrentGameState = GameState.Finished;
                    await Task.Delay(1000);
                    ResetRound();
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

        // Start a new round
        private async void StartNewRound()
        {
            isAnimatingSequence = true;
            statusMessage = "Watch the sequence...";
            CurrentGameState = GameState.Started;
            AddRandomSquare();

            foreach (var squareId in Sequence.ToList())
            {
                activeSquare = squareId;
                StateHasChanged();

                await Task.Delay(800);
                activeSquare = null;
                StateHasChanged();
                await Task.Delay(300);
            }

            statusMessage = "Your turn! Repeat the sequence.";
            isAnimatingSequence = false;
            CurrentStep = 0;
        }

        // Add a random square to the sequence
        private void AddRandomSquare()
        {
            var random = new Random();
            Sequence.Add(random.Next(1, 5));
        }

        // Check if the player's input matches the current step in the sequence
        private bool CheckPlayerInput(int input)
        {
            if (CurrentStep >= Sequence.Count || CurrentStep < 0) return false;
            return Sequence[CurrentStep] == input;
        }

        // Check if the player has completed the current sequence
        private bool IsRoundComplete() => CurrentStep >= Sequence.Count;

        // End the game (called by the End Game button)
        public void EndGame()
        {
            statusMessage = "Game ended by the player.";
            CurrentGameState = GameState.Finished;
        }

        private void ResetRound()
        {
            CurrentStep = 0;
            CurrentGameState = GameState.Waiting;
        }

        private void ResetGame()
        {
            Sequence.Clear();
            CurrentStep = 0;
            isAnimatingSequence = false;
            CurrentGameState = GameState.Waiting;
        }

        private bool IsSquareLit(int squareId) => activeSquare == squareId;

        private bool IsSquareClicked(int squareId) => clickedSquare == squareId;

        private bool DisableSquareClick() => CurrentGameState != GameState.Started;
    }
}
