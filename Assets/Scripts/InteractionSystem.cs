using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
    void OnHover();
    void OnHoverExit();
}

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private Transform m_player;
    [SerializeField] private float m_raycastRange = 100f;
    private InteractableRoot m_currentInteractable;
    private Outline m_currentOutline;
    private Vector2 m_mousePos;

    private void Update()
    {
        Ray ray = m_camera.ScreenPointToRay(m_mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, m_raycastRange))
        {
            if (!hit.collider.TryGetComponent(out InteractableChild child))
            {
                Clear();
                return;
            }

            Vector3 playerPos = m_player.position;
            Vector3 hitPos = hit.collider.transform.position;

            playerPos.y = 0;
            hitPos.y = 0;

            float distance = Vector3.Distance(playerPos, hitPos);

            // TODO read interactable distance from ScriptableObject attribute
            if (distance > 3f)
            {
                Clear();
                return;
            }

            InteractableRoot interactable = child.Root;
            interactable.TryGetComponent(out Outline outline);

            if (interactable != m_currentInteractable || outline != m_currentOutline)
            {
                Clear();

                m_currentInteractable = interactable;
                m_currentOutline = outline;

                m_currentInteractable?.OnHover();

                if (m_currentOutline)
                    m_currentOutline.enabled = true;
            }

            if (m_currentInteractable is not null && (Keyboard.current.eKey.wasPressedThisFrame ||
                                                      Mouse.current.leftButton.wasPressedThisFrame))
            {
                m_currentInteractable.Interact();
            }
        }
        else
        {
            Clear();
        }
    }

    private void Clear()
    {
        if (m_currentOutline)
            m_currentOutline.enabled = false;

        m_currentInteractable?.OnHoverExit();

        m_currentInteractable = null;
        m_currentOutline = null;
    }

    private void OnPoint(InputValue value)
    {
        m_mousePos = value.Get<Vector2>();
    }
}