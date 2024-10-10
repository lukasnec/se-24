using System.Text.Json;

namespace src.Games.FinderGame
{
    public class LevelLoader
    {
        public List<Level> LoadAllLevels(string directoryPath)
        {
            List<Level> levels = [];

            // Get all JSON files in the directory
            string[] levelFiles = Directory.GetFiles(directoryPath, "*.json");

            foreach (string file in levelFiles)
            {
                // Open the file as a stream
                using (FileStream fileStream = new(file, FileMode.Open, FileAccess.Read))
                {
                    Level level = LoadLevel(fileStream);
                    levels.Add(level);
                }
            }

            return levels;
        }
        public Level LoadLevel(Stream stream)
        {
            Level level = JsonSerializer.Deserialize<Level>(stream);
            return level;
        }
    }
}
