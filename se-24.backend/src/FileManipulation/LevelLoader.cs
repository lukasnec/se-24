using se_24.backend.src.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace se_24.backend.src.FileManipulation
{
    public class LevelLoader<T> : ILevelLoader<T> where T : class, new()
    {
        public List<T> LoadAllLevels(string directoryPath)
        {
            var levels = new ConcurrentBag<T>();

            string[] levelFiles = Directory.GetFiles(directoryPath, "*.json");

            Parallel.ForEach(levelFiles, file =>
            {
                try
                {
                    using (FileStream fileStream = new(file, FileMode.Open, FileAccess.Read))
                    {
                        T level = LoadLevel(fileStream);
                        if (level != null)
                        {
                            levels.Add(level);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                }
            });

            return levels.ToList();
        }

        public T LoadLevel(Stream stream)
        {
            try
            {
                T level = JsonSerializer.Deserialize<T>(stream);
                return level;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
