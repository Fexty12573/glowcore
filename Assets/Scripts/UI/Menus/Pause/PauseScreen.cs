using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class PauseScreen : MonoBehaviour, IMenuScreen
    {
        private const string kReturnToMenuMessage = "Return to main menu? Your progress will be saved.";
        private const string kSavedFeedbackText = "SAVED";
        private const float kSaveFeedbackSeconds = 0.9f;

        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_saveButton;
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_mainMenuButton;

        private IMenuManager m_menuManager;
        private IGameStateController m_gameState;
        private Coroutine m_saveFeedbackCoroutine;

        public MenuScreenId Id => MenuScreenId.Pause;
        public bool BlocksGameplay => true;

        public void Initialize(IMenuManager menuManager, IGameStateController gameState)
        {
            m_menuManager = menuManager;
            m_gameState = gameState;

            if (m_resumeButton != null)
                m_resumeButton.onClick.AddListener(OnResume);
            if (m_saveButton != null)
                m_saveButton.onClick.AddListener(OnSave);
            if (m_settingsButton != null)
                m_settingsButton.onClick.AddListener(OnSettings);
            if (m_mainMenuButton != null)
                m_mainMenuButton.onClick.AddListener(OnMainMenu);

            m_canvasGroup?.SetVisible(false);
        }

        public void Show() => m_canvasGroup?.SetVisible(true);

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_resumeButton != null)
                m_resumeButton.onClick.RemoveListener(OnResume);
            if (m_saveButton != null)
                m_saveButton.onClick.RemoveListener(OnSave);
            if (m_settingsButton != null)
                m_settingsButton.onClick.RemoveListener(OnSettings);
            if (m_mainMenuButton != null)
                m_mainMenuButton.onClick.RemoveListener(OnMainMenu);
        }

        private void OnResume()
        {
            m_gameState?.Resume();
            m_menuManager?.CloseAll();
        }

        private void OnSave()
        {
            m_gameState?.SaveNow();

            TextMeshProUGUI label = m_saveButton != null
                ? m_saveButton.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true)
                : null;
            if (label == null)
                return;

            if (m_saveFeedbackCoroutine != null)
                StopCoroutine(m_saveFeedbackCoroutine);
            m_saveFeedbackCoroutine = StartCoroutine(ShowSaveFeedback(label));
        }

        private IEnumerator ShowSaveFeedback(TextMeshProUGUI label)
        {
            var original = label.text;
            label.text = kSavedFeedbackText;
            yield return new WaitForSecondsRealtime(kSaveFeedbackSeconds);
            if (label != null)
                label.text = original;
            m_saveFeedbackCoroutine = null;
        }

        private void OnSettings() => m_menuManager?.Open(MenuScreenId.Settings);

        private void OnMainMenu()
        {
            var args = new ConfirmDialogArgs
            {
                Message = kReturnToMenuMessage,
                OnConfirm = () => m_gameState?.ReturnToMainMenu(),
            };
            m_menuManager?.OpenWithArgs(MenuScreenId.ConfirmDialog, args);
        }
    }
}
