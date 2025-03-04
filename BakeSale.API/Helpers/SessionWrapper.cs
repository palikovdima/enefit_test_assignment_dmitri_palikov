using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BakeSale.API.Helpers
{
    public class SessionWrapper : ISessionWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session ?? throw new InvalidOperationException("Session is not available.");

        public void SetObject<T>(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            Session.SetString(key, jsonData);
        }

        public T? GetObject<T>(string key)
        {
            var jsonData = Session.GetString(key);
            return jsonData == null ? default : JsonConvert.DeserializeObject<T>(jsonData);
        }

        public void Remove(string key)
        {
            Session.Remove(key);
        }

        public string GetString(string key)
        {
            return Session.GetString(key)!;
        }

        public void SetString(string key, string value)
        {
            Session.SetString(key, value);
        }
    }
}
