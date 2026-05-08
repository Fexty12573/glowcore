namespace GlowCore.UI.Menus
{
    public interface INewGameService
    {
        bool SaveExists { get; }
        void StartNewGame(string worldName);
    }
}
