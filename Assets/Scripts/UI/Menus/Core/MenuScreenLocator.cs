using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public class MenuScreenLocator : IMenuScreenLocator
    {
        private readonly Dictionary<MenuScreenId, IMenuScreen> m_screens = new();

        public IMenuScreen Get(MenuScreenId id)
        {
            return m_screens.TryGetValue(id, out IMenuScreen screen) ? screen : null;
        }

        public void Register(IMenuScreen screen)
        {
            if (screen == null)
                return;
            m_screens[screen.Id] = screen;
        }

        public void Unregister(MenuScreenId id)
        {
            m_screens.Remove(id);
        }
    }
}
