using UnityEngine;
using UnityEngine.InputSystem;

namespace GlowCore.UI.Menus
{
    public class PauseInputHandler : MonoBehaviour
    {
        private EscapeRouter m_router;

        public void Initialize(EscapeRouter router)
        {
            m_router = router;
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
                return;

            m_router?.Dispatch();
        }
    }
}
