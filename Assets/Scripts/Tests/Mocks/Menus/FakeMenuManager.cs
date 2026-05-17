using System;
using GlowCore.UI.Menus;

public class FakeMenuManager : IMenuManager
{
    public IMenuScreen Current { get; set; }
    public int BackCallCount { get; private set; }
    public int CloseAllCallCount { get; private set; }
    public MenuScreenId LastOpenedId { get; private set; } = MenuScreenId.None;
    public object LastOpenedArgs { get; private set; }

    public event Action<IMenuScreen> OnScreenChanged;

    public void Open(MenuScreenId id)
    {
        LastOpenedId = id;
        LastOpenedArgs = null;
    }

    public void OpenWithArgs<TArgs>(MenuScreenId id, TArgs args)
    {
        LastOpenedId = id;
        LastOpenedArgs = args;
    }

    public void Back() => BackCallCount++;

    public void CloseAll() => CloseAllCallCount++;

    public void RaiseScreenChanged() => OnScreenChanged?.Invoke(Current);
}
