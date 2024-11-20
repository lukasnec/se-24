using Bunit;
using se_24.frontend.Components.Pages;
using se_24.shared.src.Shared;
using src.Enums;

namespace se_24.tests.Tests
{
    public class FinderGameTests : TestContext
    {
        [Fact]
        public void GameState_ShouldBeWaiting_WhenComponentIsInitialized()
        {
            var component = RenderComponent<FinderGame>();
            Assert.Equal(GameState.Waiting, component.Instance.GetCurrentGameState());
        }

        [Fact]
        public void StartGame_ShouldChangeGameStateToStarted()
        {
            var component = RenderComponent<FinderGame>();

            component.Instance.StartGame();

            Assert.Equal(GameState.Started, component.Instance.GetCurrentGameState());
        }

        [Fact]
        public void ObjectClicked_ShouldIncreaseObjectsFound()
        {
            var gameObject = new GameObject
            {
                Image = "images/findergame/pixelated-santa-large.png",
                IsFound = false,
                Position = new Position { X = 50, Y = 50 }
            };
            var component = RenderComponent<FinderGame>();

            component.Instance.ObjectClicked(gameObject);

            Assert.True(gameObject.IsFound); // Object should be marked as found
            Assert.Equal(1, component.Instance.objectsFound); // One object should be found
        }

        [Fact]
        public void CalculateScore_ShouldCalculateCorrectly()
        {
            var component = RenderComponent<FinderGame>();

            // Test for "easy"
            component.Instance.selectedDifficulty = "easy";
            component.Instance.totalGivenTime = 100;
            component.Instance.totalElapsedTime = 80;
            component.Instance.CalculateScore();

            Assert.Equal(20, component.Instance.score); // Easy should multiply by 1

            // Test for "medium"
            component.Instance.selectedDifficulty = "medium";
            component.Instance.CalculateScore();
            Assert.Equal(40, component.Instance.score); // Medium should multiply by 2

            // Test for "hard"
            component.Instance.selectedDifficulty = "hard";
            component.Instance.CalculateScore();
            Assert.Equal(60, component.Instance.score); // Hard should multiply by 3
        }
    }
}