using se_24.frontend.Components.Pages;
using src.Enums;
using Bunit;

namespace se_24.tests.Tests
{
    public class BlockGameTests : TestContext
    {
        [Fact]
        public void GameState_ShouldBeWaiting_WhenComponentIsInitialized()
        {
            var component = RenderComponent<BlockGame>();

            Assert.Equal(GameState.Waiting, component.Instance.CurrentGameState);
        }

        [Fact]
        public async Task StartGame_ShouldInitializeGameStateAndResetValues()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            Assert.Equal(GameState.Started, component.Instance.CurrentGameState);
            Assert.Equal(1, component.Instance.roundNumber);
            Assert.Equal(0, component.Instance.playerStats.CorrectSequenceCount);
        }

        [Fact]
        public void GenerateNewRandomSequence_ShouldCreateSequenceOfGivenLength()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.GenerateNewRandomSequence(3);

            Assert.Equal(3, component.Instance.Sequence.Count);
            Assert.All(component.Instance.Sequence, move => Assert.InRange(move.SquareId, 1, 4));
        }

        [Fact]
        public void CheckPlayerInput_ShouldReturnTrue_WhenInputMatchesSequence()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.GenerateNewRandomSequence(3);
            var correctSquare = component.Instance.Sequence[0].SquareId;

            var result = component.Instance.CheckPlayerInput(correctSquare);

            Assert.True(result);
        }

        [Fact]
        public void CheckPlayerInput_ShouldReturnFalse_WhenInputDoesNotMatchSequence()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.GenerateNewRandomSequence(3);
            var result = component.Instance.CheckPlayerInput(999);

            Assert.False(result);
        }

        [Fact]
        public async Task OnPlayerClick_ShouldAdvanceStep_WhenInputIsCorrect()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame(); //CurrentStep = 0
            var correctSquare = component.Instance.Sequence[0].SquareId;

            await component.Instance.OnPlayerClick(correctSquare); //CurrentStep = 1 -> round ends
                                                                   //and new round starts with CurrentStep = 0

            Assert.Equal(0, component.Instance.CurrentStep);
        }

        [Fact]
        public async Task OnPlayerClick_ShouldFailAndResetGame_WhenInputIsIncorrect()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            await component.Instance.OnPlayerClick(999);

            Assert.Equal(GameState.Waiting, component.Instance.CurrentGameState);
            Assert.True(component.Instance.showFinalScore);
        }

        [Fact]
        public async Task StartNewRound_ShouldGenerateSequenceAndResetSteps()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            await component.Instance.StartNewRound();

            Assert.NotEmpty(component.Instance.Sequence);
            Assert.Equal(0, component.Instance.CurrentStep);
            Assert.False(component.Instance.showFinalScore);
        }

        [Fact]
        public void EndGame_ShouldSetGameStateToFinished()
        {
            var component = RenderComponent<BlockGame>();

            component.Instance.EndGame();

            Assert.Equal(GameState.Finished, component.Instance.CurrentGameState);
            Assert.Equal("Game ended by the player.", component.Instance.statusMessage);
        }

        [Fact]
        public void CalculateScore_ShouldUpdatePlayerScore()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.playerStats = component.Instance.playerStats with { CorrectSequenceCount = 5 };

            component.Instance.CalculateScore();

            Assert.Equal(5, component.Instance.score);
        }
    }
}