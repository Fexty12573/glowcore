namespace GlowCore.UI.Menus
{
    public sealed class NullSingleInstanceGuard : ISingleInstanceGuard
    {
        public bool TryAcquire() => true;
        public void Release() { }
        public void Dispose() { }
    }
}
