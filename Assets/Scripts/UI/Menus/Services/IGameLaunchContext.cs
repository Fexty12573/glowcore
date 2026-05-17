namespace GlowCore.UI.Menus
{
    public interface IGameLaunchContext
    {
        GameLaunchMode Mode { get; }
        string WorldName { get; }
        void SetNewGame(string worldName);
        void SetContinue();
    }
}
