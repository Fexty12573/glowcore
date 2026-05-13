using System;

namespace GlowCore.UI.Menus
{
    public class AudioBridge : IAudioBridge
    {
        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;

        public void RaiseMaster(float v) => OnMasterVolumeChanged?.Invoke(v);

        public void RaiseMusic(float v) => OnMusicVolumeChanged?.Invoke(v);

        public void RaiseSfx(float v) => OnSfxVolumeChanged?.Invoke(v);
    }
}
