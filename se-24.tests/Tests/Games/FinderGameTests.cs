using Bunit;
using se_24.frontend.Components.Pages;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Shared;
using src.Enums;
using Moq;
using System.Net;
using System.Text.Json;
using Moq.Protected;
using se_24.shared.src.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Bunit.TestDoubles;

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
                        new Level {Difficulty = "easy", GivenTime = 30, GameObjects = new List<GameObject>() },
                        new Level {Difficulty = "easy", GivenTime = 20, GameObjects = new List<GameObject>() }
                    }))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();

            component.Instance.SetDifficulty("easy");
            await component.Instance.GetGameLevels();

            Assert.Equal("easy", component.Instance.selectedDifficulty);
            Assert.NotEmpty(component.Instance.currentLevels);
            Assert.Equal(30, component.Instance.currentLevels.First().GivenTime);
        }

        [Fact]
        public async Task GetGameLevels_ApiError_ThrowsApiException()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();

            await Assert.ThrowsAsync<ApiException>(() => component.Instance.GetGameLevels());
        }

        [Fact]
        public void StartGame_SetsGameStateToStarted()
        {
            var component = RenderComponent<FinderGame>();

            component.Instance.StartGame();

            Assert.Equal(GameState.Started, component.Instance.GetCurrentGameState());
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

        [Fact]
        public async Task SaveScore_ValidScore_PostsScoreSuccessfully()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();
            component.Instance.score = 100;
            component.Instance.username = "TestUser";

            await component.Instance.SaveScore();

            Assert.Equal("Successfully saved score!", component.Instance.scoreSaveStatusMessage);
        }

        [Fact]
        public async Task SaveScore_PostFailed()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<FinderGame>();
            component.Instance.score = 100;
            component.Instance.username = "TestUser";

            await component.Instance.SaveScore();

            Assert.Equal("Failed to save score!", component.Instance.scoreSaveStatusMessage);

        }

        [Fact]
        public void GetCurrentLevel_ReturnsCurrentLevelFromList()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level {Difficulty = "easy", GivenTime = 30, GameObjects = new List<GameObject>() }
            };
            var currentLevel = component.Instance.GetCurrentLevel();
            Assert.Equal("easy", currentLevel.Difficulty);
            Assert.Equal(30, currentLevel.GivenTime);
            Assert.Empty(currentLevel.GameObjects);
        }

        [Fact]
        public void CountDownTimer_CounterDecrementsAndElapsedTimeIncrements()
        {
            var component = RenderComponent<FinderGame>();
            var mockLevel = new Level
            {
                GameObjects = new List<GameObject>
                {
                    new GameObject { Image = "Temp", IsFound = false, Position = new Position {X = 0, Y = 1 } },
                    new GameObject { Image = "Temp",  IsFound = false, Position = new Position {X = 0, Y = 1 } }
                }
            };
            component.Instance.currentLevels = new List<Level> { mockLevel };
            component.Instance.StartGame();
            component.Instance.counter = 10;
            component.Instance.totalElapsedTime = 0;

            component.Instance.CountDownTimer(null, null);

            Assert.Equal(9, component.Instance.counter);
            Assert.Equal(1, component.Instance.totalElapsedTime);
        }

        [Fact]
        public void CountDownTimer_TimerExpiresWithoutFindingAllObjects_GameStateFailed()
        {
            var component = RenderComponent<FinderGame>();
            var mockLevel = new Level
            {
                GameObjects = new List<GameObject>
                {
                    new GameObject { Image = "Temp", IsFound = false, Position = new Position {X = 0, Y = 1 } },
                    new GameObject { Image = "Temp",  IsFound = false, Position = new Position {X = 0, Y = 1 } }
                }
            };
            component.Instance.currentLevels = new List<Level> { mockLevel };
            component.Instance.StartGame();
            component.Instance.counter = 0;
            
            component.Instance.CountDownTimer(null, null);

            Assert.Equal(GameState.Failed, component.Instance.gameState);
        }

        [Fact]
        public void CountDownTimer_TimerStops_WhenObjectsFound()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.counter = 5;
            component.Instance.objectsFound = 2;

            var mockLevel = new Level
            {
                GameObjects = new List<GameObject>
                {
                    new GameObject { Image = "Temp", IsFound = true, Position = new Position {X = 0, Y = 1 } },
                    new GameObject { Image = "Temp",  IsFound = true, Position = new Position {X = 0, Y = 1 } }
                }
            };
            component.Instance.currentLevels = new List<Level> { mockLevel };

            component.Instance.CountDownTimer(null, null);

            Assert.False(FinderGame.timer.Enabled);
        }

        [Fact]
        public void ReturnToHomePage_NavigatesToRoot()
        {
            var component = RenderComponent<FinderGame>();
            var navigationManager = Services.GetRequiredService<FakeNavigationManager>();
            component.Instance.NavigationManager = navigationManager;

            component.Instance.ReturnToHomePage();

            Assert.Equal("http://localhost/", navigationManager.Uri);
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
                        new GameObject { Image = "Temp", IsFound = true, Position = new Position {X = 0, Y = 1 } },
                        new GameObject { Image = "Temp",  IsFound = true, Position = new Position {X = 0, Y = 1 } }
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
        public void LoadNextLevel_AdvancesLevelAndResetsState()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level { GivenTime = 30 },
                new Level { GivenTime = 60 }
            };
            component.Instance.gameState = GameState.Started;
            component.Instance.objectsFound = 2;

            component.Instance.LoadNextLevel();

            Assert.Equal(1, component.Instance.currentLevelIndex);
            Assert.Equal(60, component.Instance.defaultTime);
            Assert.Equal(GameState.Waiting, component.Instance.gameState);
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
                        new GameObject { Image = "Temp", IsFound = true, Position = new Position {X = 0, Y = 1 } },
                        new GameObject { Image = "Temp",  IsFound = true, Position = new Position {X = 0, Y = 1 } }
                    }
                },
                new Level
                {
                    GameObjects = new List<GameObject>
                    {
                        new GameObject { Image = "Temp", IsFound = false, Position = new Position {X = 0, Y = 1 } },
                        new GameObject { Image = "Temp",  IsFound = false, Position = new Position {X = 0, Y = 1 } }
                    }
                }
            };
            component.Instance.objectsFound = 2;

            var result = component.Instance.CheckIfAllObjectsFound();

            Assert.True(result);
            Assert.False(component.Instance.completedLevels);
        }

        [Fact]
        public void CheckIfAllObjectsFound_SetsCompletedLevelsOnLastLevel()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.currentLevels = new List<Level>
            {
                new Level
                {
                    GameObjects = new List<GameObject>
                    {
                        new GameObject { Image = "Temp", IsFound = true, Position = new Position {X = 0, Y = 1 } },
                        new GameObject { Image = "Temp",  IsFound = true, Position = new Position {X = 0, Y = 1 } }
                    }
                }
            };
            component.Instance.objectsFound = 2;

            var result = component.Instance.CheckIfAllObjectsFound();

            Assert.True(result);
            Assert.True(component.Instance.completedLevels);
        }

        [Fact]
        public void GetCurrentGameState_ReturnsCorrectState()
        {
            var component = RenderComponent<FinderGame>();
            component.Instance.gameState = GameState.Started;

            var result = component.Instance.GetCurrentGameState();

            Assert.Equal(GameState.Started, result);
        }
    }
}