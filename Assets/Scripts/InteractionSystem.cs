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
    public Camera cam;
    private IInteractable currentInteractable;
    private Outline currentOutline;

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            IInteractable interactable;
            Outline outline;

            hit.collider.TryGetComponent(out interactable);
            hit.collider.TryGetComponent(out outline);
            if (interactable != currentInteractable || outline != currentOutline)
            {
                Clear();

                currentInteractable = interactable;
                currentOutline = outline;

                currentInteractable?.OnHover();

                if (currentOutline is not null)
                    currentOutline.enabled = true;
            }

            if (currentInteractable is not null && (Keyboard.current.eKey.wasPressedThisFrame ||
                                                    Mouse.current.leftButton.wasPressedThisFrame))
            {
                currentInteractable.Interact();
            }
        }
        else
        {
            Clear();
        }
    }

    void Clear()
    {
        if (currentOutline)
            currentOutline.enabled = false;

        currentInteractable?.OnHoverExit();

        currentInteractable = null;
        currentOutline = null;
    }
}