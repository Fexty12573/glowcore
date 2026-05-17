using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class GameStateController : IGameStateController
    {
        private readonly ISaveService m_saveService;
        private readonly ISceneTransition m_sceneTransition;
        private bool m_isPaused;

        public GameStateController(ISaveService saveService, ISceneTransition sceneTransition)
        {
            m_saveService = saveService;
            m_sceneTransition = sceneTransition;
        }

        public bool IsPaused => m_isPaused;

        public void Pause()
        {
            if (m_isPaused)
                return;
            m_isPaused = true;
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            if (!m_isPaused)
                return;
            m_isPaused = false;
            Time.timeScale = 1f;
        }

        public void SaveNow() => m_saveService?.Save();

        public void ReturnToMainMenu()
        {
            m_saveService?.Save();
            Resume();
            m_sceneTransition.LoadTitleScene();
        }
    }
}
