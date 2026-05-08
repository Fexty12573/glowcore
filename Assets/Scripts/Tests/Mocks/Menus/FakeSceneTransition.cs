using GlowCore.UI.Menus;

namespace Tests.Mocks.Menus
{
    public class FakeSceneTransition : ISceneTransition
    {
        public int LoadTitleCount { get; private set; }
        public int LoadMainWorldCount { get; private set; }
        public int QuitCount { get; private set; }

        public void LoadTitleScene() => LoadTitleCount++;

        public void LoadMainWorldScene() => LoadMainWorldCount++;

        public void QuitApplication() => QuitCount++;
    }
}
