using se_24.backend.src.FileManipulation;
using se_24.shared.src.Games.ReadingGame;
using System.Text;
using System.Text.Json;

namespace se_24.tests.Tests.FileManipulation
{
    public class LevelLoaderTests
    {
        private readonly LevelLoader<ReadingLevel> _readingLevelLoader;

        public LevelLoaderTests()
        {
            _readingLevelLoader = new LevelLoader<ReadingLevel>();
        }

        [Fact]
        public void LoadAllLevels_ValidJsonFiles_ReturnsListOfLevels()
        {
            string directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryPath);

            var level1Json = JsonSerializer.Serialize(new ReadingLevel { Level = 1 });
            var level2Json = JsonSerializer.Serialize(new ReadingLevel { Level = 2 });

            File.WriteAllText(Path.Combine(directoryPath, "level1.json"), level1Json);
            File.WriteAllText(Path.Combine(directoryPath, "level2.json"), level2Json);

            var levels = _readingLevelLoader.LoadAllLevels(directoryPath);

            Assert.Equal(2, levels.Count);
            Assert.Contains(levels, level => level.Level == 1);
            Assert.Contains(levels, level => level.Level == 2);

            Directory.Delete(directoryPath, true);
        }

        [Fact]
        public void LoadAllLevels_NoJsonFiles_ReturnsEmptyList()
        {
            string directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directoryPath);

            var levels = _readingLevelLoader.LoadAllLevels(directoryPath);

            Assert.Empty(levels);

            Directory.Delete(directoryPath, true);
        }

        [Fact]
        public void LoadLevel_ValidJsonStream_ReturnsDeserializedObject()
        {
            var level = new ReadingLevel { Level = 1 };
            var json = JsonSerializer.Serialize(level);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            var deserializedLevel = _readingLevelLoader.LoadLevel(stream);

            Assert.NotNull(deserializedLevel);
            Assert.Equal(level.Level, deserializedLevel.Level);
        }

        [Fact]
        public void LoadLevel_InvalidJsonStream_ThrowsJsonException()
        {
            var invalidJson = "Invalid JSON content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidJson));

            Assert.Throws<JsonException>(() => _readingLevelLoader.LoadLevel(stream));
        }
    }
}
