using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class TitleScreen : MonoBehaviour, IMenuScreen
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Button m_newGameButton;
        [SerializeField] private Button m_continueButton;
        [SerializeField] private Button m_exitButton;
        [SerializeField] private TextMeshProUGUI m_versionLabel;

        private IMenuManager m_menuManager;
        private INewGameService m_newGameService;
        private IGameLaunchContext m_launchContext;
        private ISceneTransition m_sceneTransition;

        public MenuScreenId Id => MenuScreenId.Title;
        public bool BlocksGameplay => true;

        public void Initialize(
            IMenuManager menuManager,
            INewGameService newGameService,
            IGameLaunchContext launchContext,
            ISceneTransition sceneTransition)
        {
            m_menuManager = menuManager;
            m_newGameService = newGameService;
            m_launchContext = launchContext;
            m_sceneTransition = sceneTransition;

            if (m_newGameButton != null)
                m_newGameButton.onClick.AddListener(OnNewGame);
            if (m_continueButton != null)
                m_continueButton.onClick.AddListener(OnContinue);
            if (m_exitButton != null)
                m_exitButton.onClick.AddListener(OnExit);

            if (m_versionLabel != null)
                m_versionLabel.text = $"v{Application.version}";

            m_canvasGroup?.SetVisible(false);
        }

        public void Show()
        {
            RefreshContinueAvailability();
            m_canvasGroup?.SetVisible(true);
        }

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_newGameButton != null)
                m_newGameButton.onClick.RemoveListener(OnNewGame);
            if (m_continueButton != null)
                m_continueButton.onClick.RemoveListener(OnContinue);
            if (m_exitButton != null)
                m_exitButton.onClick.RemoveListener(OnExit);
        }

        private void RefreshContinueAvailability()
        {
            if (m_continueButton == null)
                return;

            var hasSave = m_newGameService != null && m_newGameService.SaveExists;
            m_continueButton.interactable = hasSave;
        }

        private void OnNewGame() => m_menuManager?.Open(MenuScreenId.NewGame);

        private void OnContinue()
        {
            m_launchContext?.SetContinue();
            m_sceneTransition?.LoadMainWorldScene();
        }

        private void OnExit() => m_sceneTransition?.QuitApplication();
    }
}
