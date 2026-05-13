using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public struct ConfirmDialogArgs
    {
        public string Message;
        public Action OnConfirm;
    }

    public class ConfirmDialogScreen : MonoBehaviour, IMenuScreen, IMenuArgsReceiver<ConfirmDialogArgs>
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_messageText;
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Button m_cancelButton;

        private IMenuManager m_menuManager;
        private Action m_onConfirm;

        public MenuScreenId Id => MenuScreenId.ConfirmDialog;
        public bool BlocksGameplay => true;

        public void Initialize(IMenuManager menuManager)
        {
            m_menuManager = menuManager;

            if (m_confirmButton != null)
                m_confirmButton.onClick.AddListener(OnConfirm);
            if (m_cancelButton != null)
                m_cancelButton.onClick.AddListener(OnCancel);

            m_canvasGroup?.SetVisible(false);
        }

        public void SetArgs(ConfirmDialogArgs args)
        {
            if (m_messageText != null)
                m_messageText.text = args.Message ?? string.Empty;
            m_onConfirm = args.OnConfirm;
        }

        public void Show() => m_canvasGroup?.SetVisible(true);

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_confirmButton != null)
                m_confirmButton.onClick.RemoveListener(OnConfirm);
            if (m_cancelButton != null)
                m_cancelButton.onClick.RemoveListener(OnCancel);
        }

        private void OnConfirm()
        {
            Action confirm = m_onConfirm;
            m_onConfirm = null;
            m_menuManager?.Back();
            confirm?.Invoke();
        }

        private void OnCancel()
        {
            m_onConfirm = null;
            m_menuManager?.Back();
        }
    }
}
