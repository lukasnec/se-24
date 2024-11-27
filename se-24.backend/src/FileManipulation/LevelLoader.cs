using se_24.backend.src.Interfaces;
using System.Text.Json;

namespace se_24.backend.src.FileManipulation
{
    public class LevelLoader<T> : ILevelLoader<T> where T : class, new()
    {
        public List<T> LoadAllLevels(string directoryPath)
        {
            List<T> levels = [];

            // Get all JSON files in the directory
            string[] levelFiles = Directory.GetFiles(directoryPath, "*.json");

            foreach (string file in levelFiles)
            {
                // Open the file as a stream
                using (FileStream fileStream = new(file, FileMode.Open, FileAccess.Read))
                {
                    T level = LoadLevel(fileStream);
                    levels.Add(level);
                }
            }

            return levels;
        }
        public T LoadLevel(Stream stream)
        {
            T level = JsonSerializer.Deserialize<T>(stream);
            return level;
        }
    }
}
