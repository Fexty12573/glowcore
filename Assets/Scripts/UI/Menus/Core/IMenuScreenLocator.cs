namespace GlowCore.UI.Menus
{
    public interface IMenuScreenLocator
    {
        IMenuScreen Get(MenuScreenId id);
        void Register(IMenuScreen screen);
        void Unregister(MenuScreenId id);
    }
}
