namespace GlowCore.UI.Menus
{
    public class NewGameService : INewGameService
    {
        private readonly ISaveDataGateway m_saveDataGateway;
        private readonly IGameLaunchContext m_launchContext;
        private readonly ISceneTransition m_sceneTransition;

        public NewGameService(
            ISaveDataGateway saveDataGateway,
            IGameLaunchContext launchContext,
            ISceneTransition sceneTransition)
        {
            m_saveDataGateway = saveDataGateway;
            m_launchContext = launchContext;
            m_sceneTransition = sceneTransition;
        }

        public bool SaveExists => m_saveDataGateway.Exists();

        public void StartNewGame(string worldName)
        {
            m_saveDataGateway.Delete();
            m_launchContext.SetNewGame(worldName);
            m_sceneTransition.LoadMainWorldScene();
        }
    }
}
