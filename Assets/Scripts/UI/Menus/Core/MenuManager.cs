using System;
using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public class MenuManager : IMenuManager
    {
        private readonly IMenuScreenLocator m_locator;
        private readonly Stack<IMenuScreen> m_history = new();
        private IMenuScreen m_current;

        public event Action<IMenuScreen> OnScreenChanged;

        public MenuManager(IMenuScreenLocator locator)
        {
            m_locator = locator;
        }

        public IMenuScreen Current => m_current;

        public void Open(MenuScreenId id)
        {
            IMenuScreen screen = m_locator.Get(id);
            if (screen == null)
                return;

            SwitchTo(screen, pushHistory: m_current != null && !ReferenceEquals(m_current, screen));
        }

        public void OpenWithArgs<TArgs>(MenuScreenId id, TArgs args)
        {
            IMenuScreen screen = m_locator.Get(id);
            if (screen == null)
                return;

            if (screen is IMenuArgsReceiver<TArgs> receiver)
                receiver.SetArgs(args);

            SwitchTo(screen, pushHistory: m_current != null && !ReferenceEquals(m_current, screen));
        }

        public void Back()
        {
            if (m_history.Count == 0)
            {
                CloseAll();
                return;
            }

            IMenuScreen previous = m_history.Pop();
            m_current?.Hide();
            m_current = previous;
            m_current.Show();
            OnScreenChanged?.Invoke(m_current);
        }

        public void CloseAll()
        {
            m_current?.Hide();
            m_current = null;
            m_history.Clear();
            OnScreenChanged?.Invoke(null);
        }

        private void SwitchTo(IMenuScreen screen, bool pushHistory)
        {
            if (ReferenceEquals(m_current, screen))
            {
                m_current.Show();
                return;
            }

            if (m_current != null)
            {
                if (pushHistory)
                    m_history.Push(m_current);
                m_current.Hide();
            }

            m_current = screen;
            m_current.Show();
            OnScreenChanged?.Invoke(m_current);
        }
    }
}
