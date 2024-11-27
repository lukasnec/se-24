using Bunit;
using Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using se_24.shared.src.Exceptions;
using se_24.shared.src.Games.FinderGame;
using se_24.shared.src.Games.ReadingGame;
using se_24.shared.src.Shared;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace se_24.tests.Tests.Games
{
    public class ReadingGameTests : TestContext
    {
        [Fact]
        public void OnInitializedAsync_LoadsReadingLevelsSuccessfully()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<ReadingLevel>
                    {
                        new ReadingLevel
                        {
                            Level = 1,
                            ReadingTime = 60,
                            Text = "Sample text",
                            Questions = new List<ReadingQuestion>
                            {
                                new ReadingQuestion
                                {
                                    Question = "Question",
                                    Answers = new[] { "A", "B", "C", "D" },
                                    CorrectAnswer = 1
                                }
                            }
                        }
                    }))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();

            component.WaitForState(() => component.Instance.readingLevels.Any());

            Assert.False(component.Instance.errorHappened);
            Assert.Equal("Sample text", component.Instance.text);
            Assert.Single(component.Instance.questions);
        }

        [Fact]
        public void OnInitializedAsync_HandlesApiException()
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

            var component = RenderComponent<Reading>();

            component.WaitForState(() => component.Instance.errorHappened);

            Assert.True(component.Instance.errorHappened);
            Assert.Equal("Failed loading text! Try again later.", component.Instance.errorMessage);
        }

        [Fact]
        public void AnswerClick_HandlesCorrectAndIncorrectAnswers()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion
                {
                    Question = "Sample Question?",
                    Answers = new[] { "A", "B", "C", "D" },
                    CorrectAnswer = 1
                }
            };
            component.Instance.currentQuestion = 1;

            component.InvokeAsync(() => component.Instance.AnswerClick(1));

            Assert.Equal("Correct!", component.Instance.correct);
            Assert.Equal(1, component.Instance.correctAnswersNum);

            component.InvokeAsync(() => component.Instance.AnswerClick(2));

            Assert.Equal("Incorrect!", component.Instance.correct);
            Assert.Equal(1, component.Instance.correctAnswersNum); // Still 1
        }

        [Fact]
        public void OnNextQuestion_PreparesNextQuestion()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion
                {
                    Question = "Question 1",
                    Answers = new[] { "A1", "B1", "C1", "D1" },
                    CorrectAnswer = 1
                },
                new ReadingQuestion
                {
                    Question = "Question 2",
                    Answers = new[] { "A2", "B2", "C2", "D2" },
                    CorrectAnswer = 2
                }
            };
            component.Instance.currentQuestion = 1;

            component.InvokeAsync(() => component.Instance.OnNextQuestion());

            Assert.Equal(2, component.Instance.currentQuestion);
            Assert.Equal("Question 2", component.Instance.question);
        }

        [Fact]
        public void OnEndLevel_CalculatesPercentageAndUpdatesState()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion { CorrectAnswer = 1 },
                new ReadingQuestion { CorrectAnswer = 2 },
                new ReadingQuestion { CorrectAnswer = 3 }
            };
            component.Instance.correctAnswersNum = 2;

            component.InvokeAsync(() => component.Instance.OnEndLevel());

            Assert.Equal(66.67, component.Instance.percentage);
            Assert.False(component.Instance.isQuestionsScreen);
            Assert.True(component.Instance.isEndScreen);
        }

        [Fact]
        public async Task SaveScore_SendsScoreSuccessfully()
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

            var component = RenderComponent<Reading>();
            component.Instance.username = "TestUser";
            component.Instance.score = 100;

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

            var component = RenderComponent<Reading>();
            component.Instance.username = "TestUser";
            component.Instance.score = 100;

            await component.Instance.SaveScore();

            Assert.Equal("Failed to save score!", component.Instance.scoreSaveStatusMessage);
        }
    }
}
