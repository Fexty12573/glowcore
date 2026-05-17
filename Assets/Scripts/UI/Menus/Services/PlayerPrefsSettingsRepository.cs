namespace GlowCore.UI.Menus
{
    public class PlayerPrefsSettingsRepository : ISettingsRepository
    {
        private readonly IPlayerPrefsBackend m_backend;

        public PlayerPrefsSettingsRepository(IPlayerPrefsBackend backend)
        {
            m_backend = backend;
        }

        public float GetFloat(string key, float fallback) => m_backend.GetFloat(key, fallback);

        public int GetInt(string key, int fallback) => m_backend.GetInt(key, fallback);

        public bool GetBool(string key, bool fallback) => m_backend.GetInt(key, fallback ? 1 : 0) != 0;

        public void SetFloat(string key, float value) => m_backend.SetFloat(key, value);

        public void SetInt(string key, int value) => m_backend.SetInt(key, value);

        public void SetBool(string key, bool value) => m_backend.SetInt(key, value ? 1 : 0);

        public void Save() => m_backend.Save();
    }
}
