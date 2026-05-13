using GlowCore.UI.Menus;

public class FakeSingleInstanceGuard : ISingleInstanceGuard
{
    public bool TryAcquireResult { get; set; } = true;
    public int TryAcquireCallCount { get; private set; }
    public int ReleaseCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public bool TryAcquire()
    {
        TryAcquireCallCount++;
        return TryAcquireResult;
    }

    public void Release() => ReleaseCallCount++;

    public void Dispose() => DisposeCallCount++;
}
