using GlowCore.UI.Menus;

public class FakeGameLaunchContext : IGameLaunchContext
{
    public GameLaunchMode Mode { get; private set; } = GameLaunchMode.Continue;
    public string WorldName { get; private set; } = "Player";

    public int SetNewGameCallCount { get; private set; }
    public int SetContinueCallCount { get; private set; }

    public void SetNewGame(string worldName)
    {
        SetNewGameCallCount++;
        Mode = GameLaunchMode.NewGame;
        WorldName = worldName;
    }

    public void SetContinue()
    {
        SetContinueCallCount++;
        Mode = GameLaunchMode.Continue;
    }
}
