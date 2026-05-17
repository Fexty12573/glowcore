using UnityEngine;
using UnityEngine.Audio;

namespace GlowCore.UI.Menus
{
    public class AudioMixerVolumeApplier : MonoBehaviour
    {
        private const float kMinDb = -80f;

        [SerializeField] private AudioMixer m_mixer;
        [SerializeField] private string m_masterParameter = "MasterVolume";
        [SerializeField] private string m_musicParameter = "MusicVolume";
        [SerializeField] private string m_sfxParameter = "SfxVolume";

        private IAudioBridge m_bridge;

        public void Initialize(IAudioBridge bridge)
        {
            if (m_bridge != null)
                Unsubscribe(m_bridge);

            m_bridge = bridge;

            if (m_bridge == null)
                return;

            m_bridge.OnMasterVolumeChanged += ApplyMaster;
            m_bridge.OnMusicVolumeChanged += ApplyMusic;
            m_bridge.OnSfxVolumeChanged += ApplySfx;
        }

        private void OnDestroy()
        {
            if (m_bridge != null)
                Unsubscribe(m_bridge);
        }

        private void Unsubscribe(IAudioBridge bridge)
        {
            bridge.OnMasterVolumeChanged -= ApplyMaster;
            bridge.OnMusicVolumeChanged -= ApplyMusic;
            bridge.OnSfxVolumeChanged -= ApplySfx;
        }

        private void ApplyMaster(float v) => Apply(m_masterParameter, v);

        private void ApplyMusic(float v) => Apply(m_musicParameter, v);

        private void ApplySfx(float v) => Apply(m_sfxParameter, v);

        private void Apply(string parameter, float v)
        {
            if (m_mixer == null || string.IsNullOrEmpty(parameter))
                return;

            m_mixer.SetFloat(parameter, LinearToDb(v));
        }

        private static float LinearToDb(float v)
        {
            return v <= 0.0001f ? kMinDb : Mathf.Log10(v) * 20f;
        }
    }
}
