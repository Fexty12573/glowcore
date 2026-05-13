using UnityEngine;

namespace GlowCore.UI.Menus
{
    public static class ProcessGuard
    {
        private static ISingleInstanceGuard s_activeGuard;
        private static bool s_acquired;

        public static bool TryAcquireOnce(ISingleInstanceGuard guard)
        {
            if (s_acquired)
                return true;

            if (guard == null)
                return true;

            if (!guard.TryAcquire())
            {
                guard.Dispose();
                return false;
            }

            s_activeGuard = guard;
            s_acquired = true;
            Application.quitting += ReleaseOnQuit;
            return true;
        }

        public static void ResetForTests()
        {
            if (s_activeGuard != null)
            {
                Application.quitting -= ReleaseOnQuit;
                s_activeGuard.Dispose();
            }
            s_activeGuard = null;
            s_acquired = false;
        }

        private static void ReleaseOnQuit()
        {
            Application.quitting -= ReleaseOnQuit;
            s_activeGuard?.Dispose();
            s_activeGuard = null;
            s_acquired = false;
        }
    }
}
