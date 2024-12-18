using Bunit;
using se_24.frontend.Components.Pages;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Shared;
using src.Enums;
using Moq;
using System.Net;
using System.Text.Json;
using Moq.Protected;
using Microsoft.Extensions.DependencyInjection;
using Bunit.TestDoubles;
using se_24.shared.src.Exceptions;
using Microsoft.AspNetCore.Components.Web;

namespace se_24.tests.Tests.Games
{
    public class FinderGameTests : TestContext
    {
        [Fact]
        public void OnInitialize_GameStateWaiting()
        {
            var component = RenderComponent<FinderGame>();
            Assert.Equal(GameState.Waiting, component.Instance.GetCurrentGameState());
        }

        [Fact]
        public void SetDifficulty_CorrectlySetsDifficulty()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.SetDifficulty("easy");
            Assert.Equal("easy", component.Instance.selectedDifficulty);
        }

        [Fact]
        public async Task GetGameLevels_ReturnsLevels_WithSelectedDifficulty()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<Level>
                    {
                        new Level
                        {
                            Difficulty = "easy",
                            GivenTime = 30,
                            GameObjects = new List<GameObject>
                            {
                                new GameObject { Image = "object1.png", IsFound = false, Position = new Position { X = 10, Y = 20 } },
                                new GameObject { Image = "object2.png", IsFound = false, Position = new Position { X = 30, Y = 40 } }
                            }
                        }
                    }))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();
            component.Instance.SetDifficulty("easy");
            await component.Instance.GetGameLevels();

            Assert.Equal("easy", component.Instance.selectedDifficulty);
            Assert.NotEmpty(component.Instance.currentLevels);
            Assert.Equal(30, component.Instance.currentLevels.First().GivenTime);
        }

        [Fact]
        public void ObjectClicked_IncreasesObjectsFound()
        {
            var gameObject = new GameObject
            {
                Image = "images/findergame/pixelated-santa-large.png",
                IsFound = false,
                Position = new Position { X = 50, Y = 50 }
            };
            var component = RenderComponent<FinderGame>();
            component.Instance.ObjectClicked(gameObject);

            Assert.True(gameObject.IsFound);
            Assert.Equal(1, component.Instance.objectsFound);
        }

        [Fact]
        public void CountDownTimer_TimerExpiresWithoutFindingAllObjects_GameStateFailed()
        {
            var component = RenderComponent<FinderGame>();
            var mockLevel = new Level
            {
                GameObjects = new List<GameObject>
                {
                    new GameObject { Image = "Temp1.png", IsFound = false, Position = new Position { X = 0, Y = 1 } },
                    new GameObject { Image = "Temp2.png", IsFound = false, Position = new Position { X = 0, Y = 1 } }
                }
            };
            component.Instance.currentLevels = new List<Level> { mockLevel };
            component.Instance.StartGame();
            component.Instance.counter = 0;

            component.Instance.CountDownTimer(null, null);

            Assert.Equal(GameState.Failed, component.Instance.gameState);
        }

        [Fact]
        public void ReloadLevel_ResetsGameObjectsAndState()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level
                {
                    GameObjects = new List<GameObject>
                    {
                        new GameObject { Image = "Temp1.png", IsFound = true, Position = new Position { X = 0, Y = 1 } },
                        new GameObject { Image = "Temp2.png", IsFound = true, Position = new Position { X = 0, Y = 1 } }
                    }
                }
            };
            component.Instance.gameState = GameState.Started;
            component.Instance.counter = 50;
            component.Instance.objectsFound = 2;

            component.Instance.ReloadLevel();

            Assert.All(component.Instance.currentLevels[component.Instance.currentLevelIndex].GameObjects, obj => Assert.False(obj.IsFound));
            Assert.Equal(GameState.Waiting, component.Instance.gameState);
            Assert.Equal(component.Instance.defaultTime, component.Instance.counter);
            Assert.Equal(0, component.Instance.objectsFound);
        }

        [Fact]
        public void CheckIfAllObjectsFound_ReturnsTrueWhenAllObjectsAreFound()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level
                {
                    GameObjects = new List<GameObject>
                    {
                        new GameObject { Image = "Temp1.png", IsFound = true, Position = new Position { X = 10, Y = 20 } },
                        new GameObject { Image = "Temp2.png", IsFound = true, Position = new Position { X = 30, Y = 40 } }
                    }
                }
            };

            var result = component.Instance.CheckIfAllObjectsFound();

            Assert.True(result);
        }

        [Fact]
        public void GetCurrentLevel_ReturnsCorrectLevel()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level { Difficulty = "easy", GivenTime = 30, GameObjects = new List<GameObject>() }
            };

            var currentLevel = component.Instance.GetCurrentLevel();
            Assert.Equal("easy", currentLevel.Difficulty);
            Assert.Equal(30, currentLevel.GivenTime);
        }

        [Fact]
        public void LoadNextLevel_AdvancesLevelAndResetsState()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level { GivenTime = 30 },
                new Level { GivenTime = 60 }
            };

            component.Instance.LoadNextLevel();

            Assert.Equal(1, component.Instance.currentLevelIndex);
            Assert.Equal(60, component.Instance.defaultTime);
            Assert.Equal(GameState.Waiting, component.Instance.gameState);
        }

        [Fact]
        public void ReturnToHomePage_NavigatesToHome()
        {
            var component = RenderComponent<FinderGame>();
            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            component.Instance.NavigationManager = navigationManager;

            component.Instance.ReturnToHomePage();

            Assert.Equal("http://localhost/", navigationManager.Uri);
        }

        [Fact]
        public void StartGame_HandlesEmptyLevelsGracefully()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>();
            component.InvokeAsync(() => component.Instance.StartGame());

            Assert.Equal(GameState.Failed, component.Instance.GetCurrentGameState());
            Assert.Contains("No levels available", component.Instance.errorMessage);
        }

        [Fact]
        public void UI_RendersLevelCompletePopupCorrectly()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.completedLevels = true;
            component.Instance.objectsFound = 5;
            component.InvokeAsync(() => component.Instance.CheckIfAllObjectsFound());

            var popup = component.Find("div");
            Assert.Contains("Congratulations!", popup.InnerHtml);
        }

        [Fact]
        public void Timer_EndsWhenCounterReachesZero()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.counter = 1;
            component.Instance.objectsFound = 0;

            component.Instance.CountDownTimer(null, null);

            Assert.Equal(0, component.Instance.counter);
            Assert.Equal(GameState.Failed, component.Instance.GetCurrentGameState());
        }

        [Fact]
        public async Task SaveScore_HandlesHttpErrorsGracefully()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Error saving score")
                });

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();
            component.Instance.score = 100;

            await component.Instance.SaveScore();

            Assert.Equal("Failed to save score!", component.Instance.scoreSaveStatusMessage);
        }

        [Fact]
        public async Task DifficultyButtons_TriggerGetGameLevels_WithCorrectDifficulty()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<Level>
                    {
                new Level { Difficulty = "easy", GivenTime = 30, GameObjects = new List<GameObject>() }
                    }))
                });

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();

            var easyButton = component.Find("button:contains('Easy')");
            await easyButton.ClickAsync(new MouseEventArgs());

            Assert.Equal("Easy", component.Instance.selectedDifficulty);
            Assert.NotEmpty(component.Instance.currentLevels);
        }

        [Fact]
        public async Task ErrorState_RendersErrorMessage_WhenApiFails()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("API Error"));

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();
            component.Instance.SetDifficulty("easy");

            await Assert.ThrowsAsync<ApiException>(() => component.Instance.GetGameLevels());

            component.Render();

            var errorSpan = component.Find("span");
            Assert.Equal("Failed to load levels! Try again later.", errorSpan.TextContent);
        }

        [Fact]
        public void LoadingState_DisplaysLoadingMessage()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.isLoading = true;
            component.Render();
            Assert.Contains("Loading...", component.Markup);
        }

        [Fact]
        public void StartGameButton_Appears_WhenLevelsAreLoaded()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level> { new Level { GivenTime = 30 } };
            component.Instance.isLoading = false;
            component.Render();

            var startButton = component.Find("button:contains('Start Game')");
            Assert.NotNull(startButton);
        }

        [Fact]
        public void ObjectClick_SetsObjectAsFound()
        {
            var component = RenderComponent<FinderGame>();

            var gameObject = new GameObject { Image = "Temp", IsFound = false, Position = new Position { X = 0, Y = 0 } };
            component.Instance.currentLevels = new List<Level>
            {
                new Level { GameObjects = new List<GameObject> { gameObject } }
            };

            var objectImage = component.Find("img[src='Temp']");
            objectImage.Click();

            Assert.True(gameObject.IsFound);
            Assert.Equal(1, component.Instance.objectsFound);
        }

    }
}
