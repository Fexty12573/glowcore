using System.Collections.Generic;
using GlowCore.UI.Menus;

namespace Tests.Mocks.Menus
{
    public class FakePlayerPrefsBackend : IPlayerPrefsBackend
    {
        private readonly Dictionary<string, float> m_floats = new();
        private readonly Dictionary<string, int> m_ints = new();

        public int SaveCallCount { get; private set; }

        public float GetFloat(string key, float fallback) => m_floats.TryGetValue(key, out var v) ? v : fallback;

        public int GetInt(string key, int fallback) => m_ints.TryGetValue(key, out var v) ? v : fallback;

        public void SetFloat(string key, float value) => m_floats[key] = value;

        public void SetInt(string key, int value) => m_ints[key] = value;

        public bool HasKey(string key) => m_floats.ContainsKey(key) || m_ints.ContainsKey(key);

        public void Save() => SaveCallCount++;
    }
}
