using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public class AudioSettingsCategory : ISettingsCategory
    {
        public const string kKeyMaster = "menu.audio.master";
        public const string kKeyMusic = "menu.audio.music";
        public const string kKeySfx = "menu.audio.sfx";
        public const float kDefaultMaster = 0.8f;
        public const float kDefaultMusic = 0.8f;
        public const float kDefaultSfx = 0.8f;

        private readonly List<SettingDescriptor> m_descriptors;

        public AudioSettingsCategory(ISettingsRepository repository, IAudioBridge bridge)
        {
            m_descriptors = new List<SettingDescriptor>
            {
                SettingDescriptor.Slider(kKeyMaster, "MASTER", 0f, 1f,
                    () => repository.GetFloat(kKeyMaster, kDefaultMaster),
                    v => { repository.SetFloat(kKeyMaster, v); bridge.RaiseMaster(v); }),
                SettingDescriptor.Slider(kKeyMusic, "MUSIC", 0f, 1f,
                    () => repository.GetFloat(kKeyMusic, kDefaultMusic),
                    v => { repository.SetFloat(kKeyMusic, v); bridge.RaiseMusic(v); }),
                SettingDescriptor.Slider(kKeySfx, "SFX", 0f, 1f,
                    () => repository.GetFloat(kKeySfx, kDefaultSfx),
                    v => { repository.SetFloat(kKeySfx, v); bridge.RaiseSfx(v); }),
            };
        }

        public string DisplayName => "AUDIO";
        public IReadOnlyList<SettingDescriptor> Descriptors => m_descriptors;
    }
}
