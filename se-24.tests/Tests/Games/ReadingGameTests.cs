using Bunit;
using Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using se_24.shared.src.Games.ReadingGame;
using System.Net;
using System.Text.Json;

namespace se_24.tests.Tests.Games
{
    public class ReadingGameTests : TestContext
    {
        [Fact]
        public void OnInitializedAsync_DoesNothingWhenLevelNotFound()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, new List<ReadingLevel>());
            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();
            component.WaitForState(() => !component.Instance.isReadingScreen);

            Assert.Empty(component.Instance.readingLevels);
            Assert.Equal(60, component.Instance.readingTime);
            Assert.Empty(component.Instance.text);
            Assert.Empty(component.Instance.questions);
        }

        [Fact]
        public void IsQuestionsScreen_RendersQuestionsAndHandlesClicks()
        {
            var component = RenderComponent<Reading>();

            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion
                {
                    Question = "Question 1",
                    Answers = new[] { "A1", "B1", "C1", "D1" },
                    CorrectAnswer = 1
                }
            };

            component.Instance.isQuestionsScreen = true;
            component.InvokeAsync(() => component.Instance.PrepareQuestion());

            component.Render();
            Assert.Contains("Question 1", component.Markup);
            Assert.Contains("A1", component.Markup);

            var answerButton = component.Find("button:nth-of-type(1)");
            answerButton.Click();

            Assert.Equal("Correct!", component.Instance.correct);
            Assert.True(component.Instance.isNextButtonEnabled);
        }

        [Fact]
        public void IsQuestionsScreen_NextAndEndButtonsWorkProperly()
        {
            var component = RenderComponent<Reading>();

            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion { Question = "Q1", CorrectAnswer = 1 }
            };

            component.Instance.isQuestionsScreen = true;
            component.Instance.currentQuestion = 1;

            component.InvokeAsync(() => component.Instance.AnswerClick(1));
            var nextButton = component.Find("button:contains('Next Question')");
            nextButton.Click();

            Assert.Equal(2, component.Instance.currentQuestion);

            component.InvokeAsync(() => component.Instance.AnswerClick(2));
            var endButton = component.Find("button:contains('End Level')");
            endButton.Click();

            Assert.True(component.Instance.isEndScreen);
        }

        [Fact]
        public void IsEndScreen_ShowsFinalScoreAndSavesSuccessfully()
        {
            var component = RenderComponent<Reading>();

            component.Instance.isEndScreen = true;
            component.Instance.correctAnswersNum = 3;
            component.Instance.percentage = 75;
            component.Instance.score = 300;

            component.Render();

            Assert.Contains("Game Over!", component.Markup);
            Assert.Contains("You correctly answered: 3 questions", component.Markup);
        }

        [Fact]
        public async Task FullReadingGameLifecycle_WorksAsExpected()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, new List<ReadingLevel>
            {
                new ReadingLevel
                {
                    Level = 1,
                    ReadingTime = 10,
                    Text = "Sample text",
                    Questions = new List<ReadingQuestion>
                    {
                        new ReadingQuestion { Question = "Q1", Answers = new[] { "A", "B", "C", "D" }, CorrectAnswer = 1 },
                        new ReadingQuestion { Question = "Q2", Answers = new[] { "A", "B", "C", "D" }, CorrectAnswer = 2 }
                    }
                }
            });

            var httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();
            await component.InvokeAsync(() => component.Instance.OnStartClick());

            Assert.True(component.Instance.isReadingScreen);

            component.InvokeAsync(() => component.Instance.OnReadingFinished());
            Assert.True(component.Instance.isQuestionsScreen);

            component.InvokeAsync(() => component.Instance.AnswerClick(1));
            component.InvokeAsync(() => component.Instance.OnNextQuestion());
            component.InvokeAsync(() => component.Instance.AnswerClick(2));

            Assert.True(component.Instance.isEndButtonEnabled);

            component.InvokeAsync(() => component.Instance.OnEndLevel());
            Assert.True(component.Instance.isEndScreen);
            Assert.Equal(100, component.Instance.percentage);
        }

        private Mock<HttpMessageHandler> GetMockHttpMessageHandler(HttpStatusCode statusCode, List<ReadingLevel>? levels)
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(levels ?? new List<ReadingLevel>()))
                });

            return mockHttpHandler;
        }

        [Fact]
        public async Task StartTimer_DecrementsCounterToZero()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isReadingScreen = true;

            await component.InvokeAsync(() => component.Instance.StartTimer(3));

            Assert.Equal(0, component.Instance.taskTimer);
            Assert.False(component.Instance.isReadingScreen);
        }

        [Fact]
        public void AnswerClick_IgnoresInvalidAnswers()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion { Question = "Q1", CorrectAnswer = 1 }
            };
            component.Instance.currentQuestion = 1;

            component.InvokeAsync(() => component.Instance.AnswerClick(0)); // Invalid Answer
            Assert.Equal(0, component.Instance.correctAnswersNum);

            component.InvokeAsync(() => component.Instance.AnswerClick(5)); // Out-of-Bounds Answer
            Assert.Equal(0, component.Instance.correctAnswersNum);
        }

        [Fact]
        public void QuestionsScreen_ShouldRenderCorrectly_WhenInitialized()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isQuestionsScreen = true;
            component.Render();

            Assert.Contains("Please select the correct answer.", component.Markup);
            Assert.Contains("btn btn-dark", component.Markup);
        }

        [Fact]
        public void AnswerButtons_ShouldBeDisabled_WhenIsButtonsDisabledIsTrue()
        {
            var component = RenderComponent<Reading>();

            component.Instance.isButtonsDisabled = true;
            component.Render();

            var buttons = component.FindAll("button");
            foreach (var button in buttons)
            {
                Assert.True(button.HasAttribute("disabled"));
            }
        }

        [Fact]
        public async Task StartTimer_StopsWhenInterrupted()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isReadingScreen = true;

            var timerTask = component.InvokeAsync(() => component.Instance.StartTimer(3));
            component.Instance.isReadingScreen = false; // Interrupt timer
            await timerTask;

            Assert.False(component.Instance.isReadingScreen);
            Assert.True(component.Instance.taskTimer > 0);
        }

        [Fact]
        public async Task StartTimer_CompletesSuccessfully()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isReadingScreen = true;

            await component.InvokeAsync(() => component.Instance.StartTimer(3));

            Assert.Equal(0, component.Instance.taskTimer);
            Assert.False(component.Instance.isReadingScreen);
            Assert.True(component.Instance.isQuestionsScreen);
        }

        [Fact]
        public async Task SaveScore_SavesSuccessfully()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.OK, null);
            var httpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("http://localhost") };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();
            component.Instance.score = 500;
            component.Instance.username = "Player1";

            await component.Instance.SaveScore();

            Assert.Contains("Successfully saved score!", component.Instance.scoreSaveStatusMessage);
        }

        [Fact]
        public async Task SaveScore_FailsToSave()
        {
            var mockHttpHandler = GetMockHttpMessageHandler(HttpStatusCode.BadRequest, null);
            var httpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("http://localhost") };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();
            component.Instance.score = 500;

            await component.Instance.SaveScore();

            Assert.Contains("Failed to save score!", component.Instance.scoreSaveStatusMessage);
        }

        [Fact]
        public void OnEndLevel_CalculatesPercentageAndChangesState()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion { CorrectAnswer = 1 },
                new ReadingQuestion { CorrectAnswer = 2 }
            };
            component.Instance.correctAnswersNum = 1;

            component.Instance.OnEndLevel();

            Assert.True(component.Instance.isEndScreen);
            Assert.Equal(50, component.Instance.percentage); // 1/2 correct
        }

        [Fact]
        public async Task SaveScore_HandlesFailureGracefully()
        {
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

            var httpClient = new HttpClient(mockHttpHandler.Object) { BaseAddress = new Uri("http://localhost") };
            Services.AddSingleton(httpClient);

            var component = RenderComponent<Reading>();
            component.Instance.score = 500;

            await component.Instance.SaveScore();

            Assert.Contains("Failed to save score!", component.Instance.scoreSaveStatusMessage);
        }

        [Fact]
        public void OnRestartClick_ResetsGameState()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isEndScreen = true;
            component.Instance.currentQuestion = 2;
            component.Instance.correctAnswersNum = 3;

            component.Instance.OnRestartClick();

            Assert.True(component.Instance.isStartScreen);
            Assert.False(component.Instance.isEndScreen);
            Assert.Equal(1, component.Instance.currentQuestion);
            Assert.Equal(0, component.Instance.correctAnswersNum);
        }

        [Fact]
        public void PrepareQuestion_DoesNothing_WhenQuestionsAreEmpty()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = null; // Simulate null state
            component.Instance.currentQuestion = 1;

            component.Instance.PrepareQuestion();

            Assert.Equal(string.Empty, component.Instance.question);
            Assert.Equal(string.Empty, component.Instance.answer1);
            Assert.Equal(string.Empty, component.Instance.answer2);
            Assert.Equal(string.Empty, component.Instance.answer3);
            Assert.Equal(string.Empty, component.Instance.answer4);
        }

        [Fact]
        public void AnswerClick_IgnoresWhenCurrentQuestionOutOfBounds()
        {
            var component = RenderComponent<Reading>();
            component.Instance.questions = new List<ReadingQuestion>
            {
                new ReadingQuestion { CorrectAnswer = 1 }
            };

            component.Instance.currentQuestion = 5; // Out of range
            component.Instance.AnswerClick(1);

            Assert.Equal(0, component.Instance.correctAnswersNum);
            Assert.False(component.Instance.isNextButtonEnabled);
            Assert.False(component.Instance.isEndButtonEnabled);
        }

        [Fact]
        public async Task StartTimer_HandlesMultipleInterruptions()
        {
            var component = RenderComponent<Reading>();
            component.Instance.isReadingScreen = true;

            var timerTask = component.InvokeAsync(() => component.Instance.StartTimer(5));

            // Interrupt twice
            component.Instance.isReadingScreen = false;
            await Task.Delay(1000);
            component.Instance.isReadingScreen = true;
            component.Instance.isReadingScreen = false;

            await timerTask;

            Assert.False(component.Instance.isReadingScreen);
            Assert.True(component.Instance.taskTimer > 0);
        }

        [Fact]
        public void UI_RendersCorrectly_InEachGameState()
        {
            var component = RenderComponent<Reading>();

            // Start Screen
            component.Instance.isStartScreen = true;
            component.Render();
            Assert.Contains("Start Reading Level", component.Markup);

            // Reading Screen
            component.Instance.isStartScreen = false;
            component.Instance.isReadingScreen = true;
            component.Render();
            Assert.Contains("I've already read it!", component.Markup);

            // Questions Screen
            component.Instance.isReadingScreen = false;
            component.Instance.isQuestionsScreen = true;
            component.Render();
            Assert.Contains("Please select the correct answer.", component.Markup);

            // End Screen
            component.Instance.isQuestionsScreen = false;
            component.Instance.isEndScreen = true;
            component.Render();
            Assert.Contains("Game Over!", component.Markup);
        }

    }
}
