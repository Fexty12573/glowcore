using System.Threading;

namespace GlowCore.UI.Menus
{
    public sealed class NamedMutexSingleInstanceGuard : ISingleInstanceGuard
    {
        public const string kDefaultName = "GlowCore.SingleInstance";

        private readonly string m_mutexName;
        private Mutex m_mutex;
        private bool m_owned;

        public NamedMutexSingleInstanceGuard(string mutexName)
        {
            m_mutexName = mutexName;
        }

        public bool TryAcquire()
        {
            if (m_owned)
                return true;

            m_mutex = new Mutex(false, m_mutexName);
            try
            {
                m_owned = m_mutex.WaitOne(0, false);
            }
            catch (AbandonedMutexException)
            {
                // Previous owner crashed without releasing — we now own it.
                m_owned = true;
            }

            if (!m_owned)
            {
                m_mutex.Dispose();
                m_mutex = null;
            }

            return m_owned;
        }

        public void Release()
        {
            if (m_mutex == null)
                return;

            if (m_owned)
            {
                m_mutex.ReleaseMutex();
                m_owned = false;
            }

            m_mutex.Dispose();
            m_mutex = null;
        }

        public void Dispose() => Release();
    }
}
