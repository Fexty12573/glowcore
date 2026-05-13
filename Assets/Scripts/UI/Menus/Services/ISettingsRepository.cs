namespace GlowCore.UI.Menus
{
    public interface ISettingsRepository
    {
        float GetFloat(string key, float fallback);
        int GetInt(string key, int fallback);
        bool GetBool(string key, bool fallback);
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetBool(string key, bool value);
        void Save();
    }
}
