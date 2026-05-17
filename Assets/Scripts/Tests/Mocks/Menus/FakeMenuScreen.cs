using GlowCore.UI.Menus;

public class FakeMenuScreen : IMenuScreen
{
    public FakeMenuScreen(MenuScreenId id, bool blocksGameplay = true)
    {
        Id = id;
        BlocksGameplay = blocksGameplay;
    }

    public MenuScreenId Id { get; }
    public bool BlocksGameplay { get; }

    public int ShowCallCount { get; private set; }
    public int HideCallCount { get; private set; }

    public void Show() => ShowCallCount++;

    public void Hide() => HideCallCount++;
}

public class FakeArgsMenuScreen<TArgs> : FakeMenuScreen, IMenuArgsReceiver<TArgs>
{
    public FakeArgsMenuScreen(MenuScreenId id, bool blocksGameplay = true)
        : base(id, blocksGameplay)
    {
    }

    public TArgs LastArgs { get; private set; }
    public int SetArgsCallCount { get; private set; }

    public void SetArgs(TArgs args)
    {
        LastArgs = args;
        SetArgsCallCount++;
    }
}
