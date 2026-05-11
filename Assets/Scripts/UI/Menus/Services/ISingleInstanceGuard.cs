using System;

namespace GlowCore.UI.Menus
{
    public interface ISingleInstanceGuard : IDisposable
    {
        bool TryAcquire();
        void Release();
    }
}
