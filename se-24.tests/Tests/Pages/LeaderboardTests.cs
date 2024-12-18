using Bunit;
using se_24.shared.src.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace se_24.tests.Tests.Pages
{
    public class LeaderboardTests : TestContext
    {
        private List<Score> MockScores => new()
        {
            new Score { PlayerName = "Alice", GameName = "FinderGame", Value = 100 },
            new Score { PlayerName = "Bob", GameName = "ReadingGame", Value = 200 },
            new Score { PlayerName = "Charlie", GameName = "BlockGame", Value = 150 },
        };

        private Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpStatusCode statusCode, List<Score> scores)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(scores ?? new List<Score>()))
                });
            return mockHttpMessageHandler;
        }

        [Fact]
        public void OnInitializedAsync_LoadsScoresSuccessfully()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            Assert.False(component.Instance.Scores.Count == 0);
            Assert.Contains(component.Instance.Scores, score => score.PlayerName == "Alice");
            Assert.Equal(3, component.Instance.Scores.Count);
        }

        [Fact]
        public void OnInitializedAsync_HandlesApiException()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.InternalServerError, null);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();

            component.WaitForState(() => !component.Instance.Scores.Any());
            Assert.Empty(component.Instance.Scores);
        }

        [Fact]
        public void FilteredScores_ReturnsFilteredData()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            component.Instance.selectedGameName = "FinderGame";

            var filteredScores = component.Instance.FilteredScores.ToList();
            Assert.Single(filteredScores);
            Assert.Equal("Alice", filteredScores.First().PlayerName);
        }

        [Fact]
        public void FilteredScores_ReturnsTopRanks()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Count > 0, TimeSpan.FromSeconds(10));

            component.Instance.topRanks = 2;

            var filteredScores = component.Instance.FilteredScores.ToList();
            Assert.Equal(2, filteredScores.Count);

            Assert.Equal("Bob", filteredScores.First().PlayerName);
        }

        [Fact]
        public void Leaderboard_DisplaysScoresInTable()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Markup.Contains("<table"));

            var rows = component.FindAll("tbody tr");
            Assert.Equal(3, rows.Count);
            Assert.Contains(rows.First().InnerHtml, "Alice");
            Assert.Contains(rows.First().InnerHtml, "FinderGame");

        }

        [Fact]
        public void Leaderboard_DisplaysNoDataMessageWhenEmpty()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, new List<Score>());
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => !component.Instance.Scores.Any());

            Assert.Contains("No scores to display", component.Markup);
        }

        [Fact]
        public void OnInitializedAsync_HandlesNullApiResponse()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, null); // Simulate null response
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => !component.Instance.Scores.Any());

            Assert.Empty(component.Instance.Scores);
            Assert.Contains("No scores to display", component.Markup);
        }

        [Fact]
        public void OnInitializedAsync_HandlesInvalidJsonResponse()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON")
                });

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Markup.Contains("Failed to load leaderboard"));

            Assert.Contains("Failed to load leaderboard.", component.Instance.errorMessage);
        }

        [Fact]
        public void FilteredScores_ShowsNoData_WhenNoMatchesFound()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            component.Instance.selectedGameName = "NonExistentGame";
            component.Render();

            Assert.Contains("No scores to display", component.Markup);
        }

        [Fact]
        public void FilteredScores_ReturnsAllScores_WhenTopRanksIsZero()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            component.Instance.topRanks = 0; // No filtering
            var filteredScores = component.Instance.FilteredScores.ToList();

            Assert.Equal(3, filteredScores.Count);
        }

        [Fact]
        public void FilteredScores_ReturnsAllScores_WhenTopRanksExceedsTotal()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            component.Instance.topRanks = 10; // Exceeds total scores
            var filteredScores = component.Instance.FilteredScores.ToList();

            Assert.Equal(3, filteredScores.Count);
        }

        [Fact]
        public void DropdownFilters_UpdateFilteredScores()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            var gameFilter = component.Find("#gameFilter");
            gameFilter.Change("ReadingGame");

            var filteredScores = component.Instance.FilteredScores.ToList();
            Assert.Single(filteredScores);
            Assert.Equal("ReadingGame", filteredScores.First().GameName);

            var rankFilter = component.Find("#rankFilter");
            rankFilter.Change(2);

            filteredScores = component.Instance.FilteredScores.ToList();
            Assert.Equal(2, filteredScores.Count);
        }

        [Fact]
        public void FilteredScores_CombinesFiltersCorrectly()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            // Set filters: Top 2 scores for "ReadingGame"
            component.Instance.selectedGameName = "ReadingGame";
            component.Instance.topRanks = 2;

            var filteredScores = component.Instance.FilteredScores.ToList();

            Assert.Single(filteredScores);
            Assert.Equal("ReadingGame", filteredScores.First().GameName);
        }

        [Fact]
        public void FilteredScores_HandlesNegativeTopRanks()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            component.Instance.topRanks = -5; // Invalid input
            var filteredScores = component.Instance.FilteredScores.ToList();

            Assert.Equal(3, filteredScores.Count); // Should return all scores
        }

        [Fact]
        public void Leaderboard_RendersCorrectUI_WhenFiltersAreApplied()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, MockScores);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<frontend.Components.Pages.Leaderboard>();
            component.WaitForState(() => component.Instance.Scores.Any());

            // Apply filter
            var gameFilter = component.Find("#gameFilter");
            gameFilter.Change("ReadingGame");

            // Check rendered table
            var rows = component.FindAll("tbody tr");
            Assert.Single(rows);
            Assert.Contains("ReadingGame", rows.First().InnerHtml);

            // Verify table still displays the correct column headers
            Assert.Contains("Rank", component.Markup);
            Assert.Contains("Name", component.Markup);
            Assert.Contains("Game Name", component.Markup);
            Assert.Contains("Score", component.Markup);
        }

    }
}
