using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Moq;
using se_24.frontend.Components.Pages;
using src.Enums;
using System.Net;

namespace se_24.tests.Tests.Games
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
        public async Task StartGame_ShouldResetGameAndSetStartedState()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            Assert.Equal(GameState.Started, component.Instance.CurrentGameState);
            Assert.Equal(1, component.Instance.roundNumber);
            Assert.Empty(component.Instance.Sequence);
            Assert.False(component.Instance.showFinalScore);
        }

        [Fact]
        public async Task StartNewRound_ShouldGenerateSequenceAndShowAnimation()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            await component.Instance.StartNewRound();

            Assert.NotEmpty(component.Instance.Sequence);
            Assert.False(component.Instance.isAnimatingSequence);
            Assert.Equal("Your turn! Repeat the sequence.", component.Instance.statusMessage);
        }

        [Fact]
        public async Task OnPlayerClick_ShouldAdvanceStep_WhenInputIsCorrect()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            var correctSquare = component.Instance.Sequence[0].SquareId;
            await component.Instance.OnPlayerClick(correctSquare);

            Assert.Equal(1, component.Instance.CurrentStep);
        }

        [Fact]
        public async Task OnPlayerClick_ShouldFailGame_WhenInputIsIncorrect()
        {
            var component = RenderComponent<BlockGame>();
            await component.Instance.StartGame();

            await component.Instance.OnPlayerClick(999);

            Assert.Equal(GameState.Waiting, component.Instance.CurrentGameState);
            Assert.True(component.Instance.showFinalScore);
            Assert.Equal("Wrong! Game over. Press 'Start New Round' to try again.", component.Instance.statusMessage);
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
        public void IsSquareLit_ShouldReturnTrue_WhenSquareIsActive()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.activeSquare = 2;

            Assert.True(component.Instance.IsSquareLit(2));
            Assert.False(component.Instance.IsSquareLit(3));
        }

        [Fact]
        public void IsSquareClicked_ShouldReturnTrue_WhenSquareIsClicked()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.clickedSquare = 3;

            Assert.True(component.Instance.IsSquareClicked(3));
            Assert.False(component.Instance.IsSquareClicked(2));
        }


        [Fact]
        public async Task SaveScore_ShouldDisplaySuccessMessage_WhenSaveSucceeds()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<BlockGame>();
            component.Instance.username = "TestUser";
            component.Instance.score = 100;

            await component.Instance.SaveScore();

            Assert.Equal("Successfully saved score!", component.Instance.scoreSaveStatusMessage);
        }


        [Fact]
        public void GridButtons_ShouldRenderCorrectly_BasedOnGameState()
        {
            var component = RenderComponent<BlockGame>();

            Assert.Contains("Start New Round", component.Markup);

            component.InvokeAsync(() => component.Instance.StartGame());
            component.Render();

            Assert.Contains("End Game", component.Markup);

            component.InvokeAsync(() => component.Instance.EndGame());
            component.Render();

            Assert.Contains("Reset Game", component.Markup);
        }

        [Fact]
        public void SaveScoreUI_ShouldRender_WhenShowFinalScoreIsTrue()
        {
            var component = RenderComponent<BlockGame>();
            component.Instance.showFinalScore = true;
            component.Render();

            Assert.Contains("Your score:", component.Markup);
            Assert.Contains("Save score", component.Markup);
        }

        [Fact]
        public async Task FullGameLifecycle_ShouldWorkCorrectly()
        {
            var component = RenderComponent<BlockGame>();

            await component.Instance.StartGame();
            Assert.Equal(GameState.Started, component.Instance.CurrentGameState);

            foreach (var move in component.Instance.Sequence.ToList())
            {
                await component.Instance.OnPlayerClick(move.SquareId);
            }


            Assert.Equal(1, component.Instance.playerStats.CorrectSequenceCount);

            component.InvokeAsync(() => component.Instance.EndGame());
            Assert.Equal(GameState.Finished, component.Instance.CurrentGameState);
            Assert.Equal("Game ended by the player.", component.Instance.statusMessage);
        }
    }
}
