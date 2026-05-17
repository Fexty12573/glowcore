using System;

namespace GlowCore.UI.Menus
{
    public interface IAudioBridge
    {
        event Action<float> OnMasterVolumeChanged;
        event Action<float> OnMusicVolumeChanged;
        event Action<float> OnSfxVolumeChanged;
        void RaiseMaster(float v);
        void RaiseMusic(float v);
        void RaiseSfx(float v);
    }
}
