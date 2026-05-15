using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class PauseScreen : MonoBehaviour, IMenuScreen
    {
        private const string kReturnToMenuMessage = "Return to main menu? Your progress will be saved.";
        private const float kSaveFeedbackSeconds = 0.9f;

        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_saveButton;
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_mainMenuButton;

        [Header("Save Feedback")]
        [SerializeField] private Sprite m_savedFeedbackSprite;

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

        public void Hide()
        {
            // EventSystem keeps the last-clicked button as currentSelectedGameObject; with
            // ColorTint that paints it in the selected color the next time the menu opens.
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
            m_canvasGroup?.SetVisible(false);
        }

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

            if (m_saveButton == null || m_savedFeedbackSprite == null)
                return;

            Image img = m_saveButton.GetComponent<Image>();
            if (img == null)
                return;

            if (m_saveFeedbackCoroutine != null)
                StopCoroutine(m_saveFeedbackCoroutine);
            m_saveFeedbackCoroutine = StartCoroutine(ShowSaveFeedback(img));
        }

        private IEnumerator ShowSaveFeedback(Image img)
        {
            Sprite original = img.sprite;
            img.sprite = m_savedFeedbackSprite;
            yield return new WaitForSecondsRealtime(kSaveFeedbackSeconds);
            if (img != null)
                img.sprite = original;
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
