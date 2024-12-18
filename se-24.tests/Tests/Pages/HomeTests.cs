using Bunit;
using se_24.frontend.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using System.Net;

namespace se_24.tests.Tests.Pages
{
    public class HomeTests : TestContext
    {
        private Mock<HttpMessageHandler> GetMockHttpHandler(HttpStatusCode statusCode, string responseContent)
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });
            return mockHttpHandler;
        }

        [Fact]
        public void HomeComponent_ShowsLoadingInitially()
        {
            var mockHttpHandler = GetMockHttpHandler(HttpStatusCode.OK, "0");
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();

            component.MarkupMatches(@"
                <div class=""text-center"">
                    <span>Loading...</span>
                </div>
            ");
        }

        [Fact]
        public void HomeComponent_LoadsLevelsSuccessfully()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("10") }) // FinderLevels
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("5") }); // ReadingLevels

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            Assert.Equal(10, component.Instance.FinderLevels);
            Assert.Equal(5, component.Instance.ReadingLevels);
            component.Markup.Contains("10 Levels");
            component.Markup.Contains("5 Levels");
        }

        [Fact]
        public void HomeComponent_ShowsError_WhenApiFails()
        {
            var mockHttpHandler = GetMockHttpHandler(HttpStatusCode.InternalServerError, string.Empty);
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            Assert.True(component.Instance.errorHappened);
            Assert.Contains("Failed to load levels", component.Markup);
        }

        [Fact]
        public void HomeComponent_HandlesEmptyResponse_Gracefully()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("") }) // Empty FinderLevels
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("") }); // Empty ReadingLevels

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            Assert.True(component.Instance.errorHappened);
            Assert.Contains("API returned an empty response", component.Markup);
        }

        [Fact]
        public void HomeComponent_ShowsError_WhenOneApiCallFails()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("10") }) // FinderLevels OK
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError }); // ReadingLevels Fails

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            Assert.True(component.Instance.errorHappened);
            Assert.Contains("Failed to load levels", component.Markup);
            Assert.Equal(10, component.Instance.FinderLevels);
            Assert.Equal(0, component.Instance.ReadingLevels);
        }

        [Fact]
        public void HomeComponent_HandlesInvalidApiResponse()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("NotANumber") }) // Invalid FinderLevels
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("5") }); // Valid ReadingLevels

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            Assert.True(component.Instance.errorHappened);
            Assert.Contains("Failed to load levels", component.Markup);
            Assert.Equal(0, component.Instance.FinderLevels); // Parsing failed
            Assert.Equal(0, component.Instance.ReadingLevels); // Error stops execution
        }

        [Fact]
        public void HomeComponent_RendersGameLinksCorrectly()
        {
            var mockHttpHandler = GetMockHttpHandler(HttpStatusCode.OK, "10");
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Home>();
            component.WaitForState(() => !component.Instance.isLoading);

            var links = component.FindAll("a.btn-dark");
            Assert.Equal(3, links.Count); // Three game buttons rendered

            Assert.Contains("/findergame", links[0].GetAttribute("href"));
            Assert.Contains("Find The Objects", links[0].TextContent);

            Assert.Contains("/reading", links[1].GetAttribute("href"));
            Assert.Contains("Reading Game", links[1].TextContent);

            Assert.Contains("/blockgame", links[2].GetAttribute("href"));
            Assert.Contains("Press the Block", links[2].TextContent);
        }

    }
}
