using src.Games.ReadingGame;
using se_24.shared.src.Games.ReadingGame;

namespace se_24.tests.Tests.Extensions
{
    public class ReadingExtensionsTests
    {
        [Fact]
        public void GetCorrectPercentage_ReturnsZeroWhenNoQuestions()
        {
            var questions = new List<ReadingQuestion>();
            var result = questions.GetCorrectPercentage(5);
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetCorrectPercentage_CalculatesCorrectly()
        {
            var questions = new List<ReadingQuestion>
            {
                new ReadingQuestion(),
                new ReadingQuestion(),
                new ReadingQuestion()
            };
            var result = questions.GetCorrectPercentage(2);
            Assert.Equal(66.67, result, 2);
        }

        [Fact]
        public void GetCorrectPercentage_ReturnsZeroWhenNoCorrectAnswers()
        {
            var questions = new List<ReadingQuestion>
            {
                new ReadingQuestion(),
                new ReadingQuestion(),
                new ReadingQuestion()
            };
            var result = questions.GetCorrectPercentage(0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetCorrectPercentage_ReturnsHundredWhenAllCorrect()
        {
            var questions = new List<ReadingQuestion>
            {
                new ReadingQuestion(),
                new ReadingQuestion(),
                new ReadingQuestion()
            };
            var result = questions.GetCorrectPercentage(3);
            Assert.Equal(100, result);
        }
    }
}
