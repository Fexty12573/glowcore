namespace GlowCore.UI.Menus
{
    public class SettingsEscapeConsumer : IEscapeConsumer
    {
        public const int kPriority = 10;

        private readonly IMenuManager m_menuManager;
        private readonly SettingsScreen m_settingsScreen;

        public SettingsEscapeConsumer(IMenuManager menuManager, SettingsScreen settingsScreen)
        {
            m_menuManager = menuManager;
            m_settingsScreen = settingsScreen;
        }

        public int Priority => kPriority;

        public bool TryConsumeEscape()
        {
            if (m_menuManager == null || m_settingsScreen == null)
                return false;

            if (m_menuManager.Current == null || m_menuManager.Current.Id != MenuScreenId.Settings)
                return false;

            m_settingsScreen.TryLeave();
            return true;
        }
    }
}
