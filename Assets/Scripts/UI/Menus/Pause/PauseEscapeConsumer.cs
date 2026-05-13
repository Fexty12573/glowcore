namespace GlowCore.UI.Menus
{
    public class PauseEscapeConsumer : IEscapeConsumer
    {
        public const int kPriority = 0;

        private readonly IMenuManager m_menuManager;
        private readonly IGameStateController m_gameState;

        public PauseEscapeConsumer(IMenuManager menuManager, IGameStateController gameState)
        {
            m_menuManager = menuManager;
            m_gameState = gameState;
        }

        public int Priority => kPriority;

        public bool TryConsumeEscape()
        {
            if (m_menuManager == null || m_gameState == null)
                return false;

            IMenuScreen current = m_menuManager.Current;

            if (current == null)
            {
                m_gameState.Pause();
                m_menuManager.Open(MenuScreenId.Pause);
                return true;
            }

            if (current.Id == MenuScreenId.Pause)
            {
                m_gameState.Resume();
                m_menuManager.CloseAll();
                return true;
            }

            // Don't auto-back from confirm; the user has to click confirm/cancel.
            if (current.Id == MenuScreenId.ConfirmDialog)
                return true;

            m_menuManager.Back();
            return true;
        }
    }
}
