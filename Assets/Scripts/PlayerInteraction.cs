using UnityEngine;
using UnityEngine.InputSystem;
using GlowCore.World;

namespace GlowCore.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        // Instance Fields
        [SerializeField] private Camera m_camera;
        [SerializeField] private float m_interactRange = 5f;

        // Public Methods
        public void HandleInteract()
        {
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, m_interactRange))
            {
                Debug.Log("Raycast hit nothing");
                return;
            }

            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            Fire fire = hit.collider.GetComponent<Fire>();
            if (fire == null)
            {
                Debug.Log("No Fire component on: " + hit.collider.gameObject.name);
                return;
            }

            IInventory inventory = GetComponent<IInventory>();
            if (inventory == null)
            {
                Debug.Log("No IInventory found");
                return;
            }

            int wood = inventory.RemoveAllWood();
            Debug.Log("Feeding " + wood + " wood to fire");
            fire.FeedWood(wood);
        }

        // Private Methods
        private void OnInteract()
        {
            Debug.Log("OnInteract called");
            HandleInteract();
        }
    }
}
