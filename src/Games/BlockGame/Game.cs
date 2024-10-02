using System;
using System.Collections.Generic;

namespace src.Games.BlockGame
{
    public class Game
    {
        public List<int> Sequence { get; private set; } = new List<int>();
        public int CurrentStep { get; set; }

        // Add a random number (1-4 representing squares) to the sequence
        public void AddRandomSquare()
        {
            var random = new Random();
            Sequence.Add(random.Next(1, 5)); // Randomly add a square (1 to 4)
        }

        // Check if the player's input is correct
        public bool CheckPlayerInput(int input)
        {
            // Ensure the CurrentStep is within bounds before checking the input
            if (CurrentStep >= Sequence.Count || CurrentStep < 0)
            {
                // This means we're trying to access an invalid step, reset the game state
                return false;
            }

            // Validate player's input against the current step in the sequence
            if (Sequence[CurrentStep] == input)
            {
                CurrentStep++; // Move to the next step if correct
                return true; // Input is correct
            }

            return false; // Input is incorrect
        }

        // Check if the player has completed the current sequence
        public bool IsRoundComplete()
        {
            return CurrentStep >= Sequence.Count; // Return true if player completed the round
        }

        // Reset the round (prepare for the next round by resetting the step counter)
        public void ResetRound()
        {
            CurrentStep = 0; // Reset the step counter for the next round
        }

        // Completely reset the game (start from scratch)
        public void ResetGame()
        {
            Sequence.Clear(); // Clear the sequence
            CurrentStep = 0;  // Reset the step counter
        }
    }
}
