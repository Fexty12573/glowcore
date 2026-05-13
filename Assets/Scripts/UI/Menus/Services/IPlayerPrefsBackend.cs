namespace GlowCore.UI.Menus
{
    public interface IPlayerPrefsBackend
    {
        float GetFloat(string key, float fallback);
        int GetInt(string key, int fallback);
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        bool HasKey(string key);
        void Save();
    }

    public class UnityPlayerPrefsBackend : IPlayerPrefsBackend
    {
        public float GetFloat(string key, float fallback) => UnityEngine.PlayerPrefs.GetFloat(key, fallback);
        public int GetInt(string key, int fallback) => UnityEngine.PlayerPrefs.GetInt(key, fallback);
        public void SetFloat(string key, float value) => UnityEngine.PlayerPrefs.SetFloat(key, value);
        public void SetInt(string key, int value) => UnityEngine.PlayerPrefs.SetInt(key, value);
        public bool HasKey(string key) => UnityEngine.PlayerPrefs.HasKey(key);
        public void Save() => UnityEngine.PlayerPrefs.Save();
    }
}
