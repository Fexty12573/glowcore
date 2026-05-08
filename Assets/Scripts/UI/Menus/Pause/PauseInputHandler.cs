using UnityEngine;
using UnityEngine.InputSystem;

namespace GlowCore.UI.Menus
{
    public class PauseInputHandler : MonoBehaviour
    {
        private EscapeRouter m_router;
        private InputAction m_action;

        public void Initialize(EscapeRouter router, InputAction pauseAction)
        {
            m_router = router;
            m_action = pauseAction;

            if (m_action == null)
                return;

            m_action.performed += OnPerformed;
            m_action.Enable();
        }

        private void OnDestroy()
        {
            if (m_action == null)
                return;

            m_action.performed -= OnPerformed;
            // Don't Disable/Dispose — the action belongs to the shared InputActionAsset.
            m_action = null;
        }

        private void OnPerformed(InputAction.CallbackContext _) => m_router?.Dispatch();
    }
}
