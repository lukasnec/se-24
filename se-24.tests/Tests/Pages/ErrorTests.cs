using Bunit;
using se_24.frontend.Components.Pages;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Diagnostics;

namespace se_24.tests.Tests.Pages
{
    public class ErrorTests : TestContext
    {
        [Fact]
        public void ErrorPage_ShowsRequestId_WhenAvailable()
        {
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(ctx => ctx.TraceIdentifier).Returns("TestRequestID");

            var activity = new Activity("TestActivity");
            activity.Start();
            Activity.Current = activity;

            var component = RenderComponent<Error>(parameters =>
                parameters.AddCascadingValue(httpContextMock.Object)
            );

            component.Markup.Contains("Request ID:");
            component.Markup.Contains("TestRequestID");

            activity.Stop();
        }

        [Fact]
        public void ErrorPage_HidesRequestId_WhenNotAvailable()
        {
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(ctx => ctx.TraceIdentifier).Returns(string.Empty);

            var component = RenderComponent<Error>(parameters =>
                parameters.AddCascadingValue(httpContextMock.Object)
            );

            Assert.DoesNotContain("Request ID:", component.Markup);
        }

        [Fact]
        public void ErrorPage_RendersStaticContentCorrectly()
        {
            var component = RenderComponent<Error>();

            component.Markup.Contains("Error.");
            component.Markup.Contains("An error occurred while processing your request.");
            component.Markup.Contains("Development Mode");
        }
    }
}
