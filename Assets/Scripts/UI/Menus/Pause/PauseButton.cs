using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private Button m_button;

        private IGameStateController m_gameState;
        private IMenuManager m_menuManager;

        public void Initialize(IGameStateController gameState, IMenuManager menuManager)
        {
            m_gameState = gameState;
            m_menuManager = menuManager;

            if (m_button != null)
                m_button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (m_button != null)
                m_button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (m_gameState == null || m_menuManager == null)
                return;
            if (m_gameState.IsPaused)
                return;

            m_gameState.Pause();
            m_menuManager.Open(MenuScreenId.Pause);
        }
    }
}
