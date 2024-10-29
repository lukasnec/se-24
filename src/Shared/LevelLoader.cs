using System.Text.Json;

namespace src.Shared
{
    public class LevelLoader
    {
        public List<T> LoadAllLevels<T>(string directoryPath)
        {
            List<T> levels = [];

            // Get all JSON files in the directory
            string[] levelFiles = Directory.GetFiles(directoryPath, "*.json");

            foreach (string file in levelFiles)
            {
                // Open the file as a stream
                using (FileStream fileStream = new(file, FileMode.Open, FileAccess.Read))
                {
                    T level = LoadLevel<T>(fileStream);
                    levels.Add(level);
                }
            }

            return levels;
        }
        public T LoadLevel<T>(Stream stream)
        {
            T level = JsonSerializer.Deserialize<T>(stream);
            return level;
        }
    }
}
