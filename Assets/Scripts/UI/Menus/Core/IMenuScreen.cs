namespace GlowCore.UI.Menus
{
    public interface IMenuScreen
    {
        MenuScreenId Id { get; }
        bool BlocksGameplay { get; }
        void Show();
        void Hide();
    }
}
