using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public struct UnsavedChangesDialogArgs
    {
        public string Message;
        public Action OnApply;
        public Action OnDiscard;
    }

    public class UnsavedChangesDialogScreen : MonoBehaviour, IMenuScreen, IMenuArgsReceiver<UnsavedChangesDialogArgs>
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_messageText;
        [SerializeField] private Button m_applyButton;
        [SerializeField] private Button m_discardButton;
        [SerializeField] private Button m_cancelButton;

        private IMenuManager m_menuManager;
        private Action m_onApply;
        private Action m_onDiscard;

        public MenuScreenId Id => MenuScreenId.UnsavedChanges;
        public bool BlocksGameplay => true;

        public void Initialize(IMenuManager menuManager)
        {
            m_menuManager = menuManager;

            if (m_applyButton != null)
                m_applyButton.onClick.AddListener(OnApply);
            if (m_discardButton != null)
                m_discardButton.onClick.AddListener(OnDiscard);
            if (m_cancelButton != null)
                m_cancelButton.onClick.AddListener(OnCancel);

            m_canvasGroup?.SetVisible(false);
        }

        public void SetArgs(UnsavedChangesDialogArgs args)
        {
            if (m_messageText != null && !string.IsNullOrEmpty(args.Message))
                m_messageText.text = args.Message;
            m_onApply = args.OnApply;
            m_onDiscard = args.OnDiscard;
        }

        public void Show() => m_canvasGroup?.SetVisible(true);

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_applyButton != null)
                m_applyButton.onClick.RemoveListener(OnApply);
            if (m_discardButton != null)
                m_discardButton.onClick.RemoveListener(OnDiscard);
            if (m_cancelButton != null)
                m_cancelButton.onClick.RemoveListener(OnCancel);
        }

        private void OnApply()
        {
            Action apply = m_onApply;
            ClearCallbacks();
            m_menuManager?.Back();
            apply?.Invoke();
        }

        private void OnDiscard()
        {
            Action discard = m_onDiscard;
            ClearCallbacks();
            m_menuManager?.Back();
            discard?.Invoke();
        }

        private void OnCancel()
        {
            ClearCallbacks();
            m_menuManager?.Back();
        }

        private void ClearCallbacks()
        {
            m_onApply = null;
            m_onDiscard = null;
        }
    }
}
