namespace GlowCore.UI.Menus
{
    public interface IMenuArgsReceiver<TArgs>
    {
        void SetArgs(TArgs args);
    }
}
