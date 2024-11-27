using se_24.shared.src.Utilities;

namespace se_24.tests.Tests.Utilities
{
    public class UsernameGeneratorTests
    {
        [Fact]
        public void GenerateGuestName_GeneratesWithSixDigitNumber()
        {
            UsernameGenerator usernameGenerator = new();
            string name = usernameGenerator.GenerateGuestName();
            Assert.Equal(12, name.Length);
            string number = name.Substring(6, 6);
            int parsedNumber = int.Parse(number);
            Assert.True(parsedNumber >= 100000 && parsedNumber < 1000000);
        }
    }
}
