namespace GlowCore.UI.Menus
{
    public class EndingEscapeConsumer : IEscapeConsumer
    {
        // Highest priority so it intercepts Esc regardless of any other open screen.
        public const int kPriority = 1000;

        private readonly IMenuManager m_menuManager;

        public EndingEscapeConsumer(IMenuManager menuManager)
        {
            m_menuManager = menuManager;
        }

        public int Priority => kPriority;

        public bool TryConsumeEscape()
        {
            IMenuScreen current = m_menuManager?.Current;
            return current != null && current.Id == MenuScreenId.Ending;
        }
    }
}
