using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BakeSale.Tests.Mocks
{
    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value!);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public string Id => "mock_session";

        public bool IsAvailable => true;

        public void Clear() => _sessionStorage.Clear();

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

}
