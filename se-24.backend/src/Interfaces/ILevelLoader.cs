namespace se_24.backend.src.Interfaces
{
    public interface ILevelLoader<T> where T : class, new()
    {
        List<T> LoadAllLevels(string directoryPath);
        T LoadLevel(Stream stream);
    }
}
