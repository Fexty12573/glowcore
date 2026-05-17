using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class NewGameScreen : MonoBehaviour, IMenuScreen
    {
        public const int kMaxNameByteLength = 31;
        private const string kConfirmMessage = "This will delete your current save.";

        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TMP_InputField m_nameInput;
        [SerializeField] private Button m_startButton;
        [SerializeField] private Button m_backButton;
        [SerializeField] private TextMeshProUGUI m_validationText;

        private IMenuManager m_menuManager;
        private INewGameService m_newGameService;

        public MenuScreenId Id => MenuScreenId.NewGame;
        public bool BlocksGameplay => true;

        public void Initialize(IMenuManager menuManager, INewGameService newGameService)
        {
            m_menuManager = menuManager;
            m_newGameService = newGameService;

            if (m_startButton != null)
                m_startButton.onClick.AddListener(OnStart);
            if (m_backButton != null)
                m_backButton.onClick.AddListener(OnBack);
            if (m_nameInput != null)
                m_nameInput.onValueChanged.AddListener(OnNameChanged);

            m_canvasGroup?.SetVisible(false);
            UpdateStartButton(string.Empty);
        }

        public void Show()
        {
            if (m_nameInput != null)
            {
                m_nameInput.text = string.Empty;
                m_nameInput.ActivateInputField();
            }
            UpdateStartButton(string.Empty);
            m_canvasGroup?.SetVisible(true);
        }

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_startButton != null)
                m_startButton.onClick.RemoveListener(OnStart);
            if (m_backButton != null)
                m_backButton.onClick.RemoveListener(OnBack);
            if (m_nameInput != null)
                m_nameInput.onValueChanged.RemoveListener(OnNameChanged);
        }

        private void OnNameChanged(string raw) => UpdateStartButton(raw);

        private void UpdateStartButton(string raw)
        {
            var trimmed = (raw ?? string.Empty).Trim();
            var byteLen = Encoding.UTF8.GetByteCount(trimmed);
            var valid = trimmed.Length > 0 && byteLen <= kMaxNameByteLength;

            if (m_startButton != null)
                m_startButton.interactable = valid;

            if (m_validationText != null)
            {
                if (trimmed.Length == 0)
                    m_validationText.text = string.Empty;
                else if (byteLen > kMaxNameByteLength)
                    m_validationText.text = "Name is too long.";
                else
                    m_validationText.text = string.Empty;
            }
        }

        private void OnStart()
        {
            if (m_newGameService == null || m_nameInput == null)
                return;

            var trimmed = (m_nameInput.text ?? string.Empty).Trim();
            if (trimmed.Length == 0)
                return;

            if (m_newGameService.SaveExists)
            {
                var args = new ConfirmDialogArgs
                {
                    Message = kConfirmMessage,
                    OnConfirm = () => m_newGameService.StartNewGame(trimmed),
                };
                m_menuManager?.OpenWithArgs(MenuScreenId.ConfirmDialog, args);
                return;
            }

            m_newGameService.StartNewGame(trimmed);
        }

        private void OnBack() => m_menuManager?.Back();
    }
}
