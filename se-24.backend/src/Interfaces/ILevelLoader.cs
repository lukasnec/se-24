namespace se_24.backend.src.Interfaces
{
    public interface ILevelLoader
    {
        List<T> LoadAllLevels<T>(string directoryPath);
        T LoadLevel<T>(Stream stream);
    }
}
