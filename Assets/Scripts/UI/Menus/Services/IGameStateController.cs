namespace GlowCore.UI.Menus
{
    public interface IGameStateController
    {
        bool IsPaused { get; }
        void Pause();
        void Resume();
        void SaveNow();
        void ReturnToMainMenu();
    }
}
