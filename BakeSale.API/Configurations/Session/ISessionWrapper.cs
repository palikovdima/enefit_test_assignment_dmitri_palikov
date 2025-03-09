namespace API.Configurations.Session
{
    public interface ISessionWrapper
    {
        T GetObject<T>(string key);
        void SetObject<T>(string key, T value);
        string GetString(string key);
        void SetString(string key, string value);
        void Remove(string key);
    }
}
