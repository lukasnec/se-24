using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace se_24.Components.Pages // Adjust the namespace according to your project structure
{
    public partial class BlockGame : ComponentBase
    {
        private List<int> Sequence { get; set; } = new List<int>(); // Stores the sequence of squares
        private int CurrentStep { get; set; } = 0; // Tracks the current step in the sequence
        private string statusMessage = "Press 'Start New Round' to begin."; // Status message shown to the player
        private bool isAnimatingSequence = false; // Prevent player input during animation
        private int? activeSquare = null; // Holds the ID of the lit square
        private int? clickedSquare = null; // Holds the ID of the square the player clicked on

        // Called when the player clicks on a square
        private async void OnPlayerClick(int squareId)
        {
            if (isAnimatingSequence) return; // Ignore clicks during sequence animation

            clickedSquare = squareId; // Set the clicked square
            StateHasChanged(); // Trigger UI update to reflect the click

            await Task.Delay(500); // Delay to show the square press
            clickedSquare = null; // Reset clicked square after delay
            StateHasChanged(); // Update UI to remove the highlight

            if (CheckPlayerInput(squareId)) // Check if the input is correct
            {
                CurrentStep++; // Increment the player's progress

                if (IsRoundComplete()) // Check if the round is complete
                {
                    statusMessage = "Correct! Starting next round...";
                    await Task.Delay(1000); // Delay before starting next round
                    ResetRound();
                    StartNewRound();
                }
            }
            else
            {
                // Wrong input, reset the game
                statusMessage = "Wrong! Game over. Press 'Start New Round' to try again.";
                ResetGame(); // Reset game state
            }
        }

        // Start a new round
        private async void StartNewRound()
        {
            isAnimatingSequence = true; // Disable player input during animation
            statusMessage = "Watch the sequence...";
            AddRandomSquare(); // Add a new random square to the sequence

            foreach (var squareId in Sequence.ToList()) // Show the sequence
            {
                activeSquare = squareId; // Highlight current square
                StateHasChanged(); // Update UI

                await Task.Delay(800); // Keep square lit for 800ms
                activeSquare = null; // Turn off the light
                StateHasChanged(); // Update UI
                await Task.Delay(300); // 300ms before lighting next square
            }

            statusMessage = "Your turn! Repeat the sequence.";
            isAnimatingSequence = false; // Re-enable player input
            CurrentStep = 0; // Reset player's step for new round
        }

        // Add a random square (1-4) to the sequence
        private void AddRandomSquare()
        {
            var random = new Random();
            Sequence.Add(random.Next(1, 5)); // Random number between 1 and 4
        }

        // Check if the player's input matches the current step in the sequence
        private bool CheckPlayerInput(int input)
        {
            if (CurrentStep >= Sequence.Count || CurrentStep < 0)
            {
                return false; // Invalid step, fail
            }

            return Sequence[CurrentStep] == input; // Compare input with the expected step
        }

        // Check if the player has completed the current sequence
        private bool IsRoundComplete()
        {
            return CurrentStep >= Sequence.Count; // Player completed the sequence
        }

        // Reset the round, prepare for the next one
        private void ResetRound()
        {
            CurrentStep = 0; // Reset player's progress
        }

        // Reset the game entirely
        private void ResetGame()
        {
            Sequence.Clear(); // Clear the sequence
            CurrentStep = 0; // Reset progress
            isAnimatingSequence = false; // Allow input
        }

        // Utility to check if a square is lit
        private bool IsSquareLit(int squareId)
        {
            return activeSquare == squareId; // Return if the square is the one currently lit
        }

        // Utility to check if a square is clicked
        private bool IsSquareClicked(int squareId)
        {
            return clickedSquare == squareId; // Return if the square was clicked
        }
    }
}
