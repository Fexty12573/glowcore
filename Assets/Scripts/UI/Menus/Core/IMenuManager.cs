using System;

namespace GlowCore.UI.Menus
{
    public interface IMenuManager
    {
        IMenuScreen Current { get; }
        void Open(MenuScreenId id);
        void OpenWithArgs<TArgs>(MenuScreenId id, TArgs args);
        void Back();
        void CloseAll();
        event Action<IMenuScreen> OnScreenChanged;
    }
}
