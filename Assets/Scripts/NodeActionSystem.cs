using System;
using GlowCore.World;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
}

public class NodeActionSystem : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private Transform m_player;
    [SerializeField] private float m_raycastRange = 100f;

    private static Node s_currentNode;
    private static Outline s_currentOutline;
    private static Vector2Int? s_currentTile;
    private static Vector2 s_mousePos;

    // both can also be called with null to notify that no Node is hovered over.
    public static event Action<Node> OnChangeSelectedNode;
    public static event Action<Vector2Int?> OnChangeSelectedTile;

    public static void OnHandItemChange()
    {
        OnChangeSelectedNode?.Invoke(s_currentNode);
        OnChangeSelectedTile?.Invoke(s_currentTile);
    }
    private void Update()
    {
        Ray ray = m_camera.ScreenPointToRay(s_mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, m_raycastRange))
        {
            UpdateHoverNode(hit);
            UpdateHoverTile(hit);
        }
        else
        {
            Clear();
        }
    }

    private void UpdateHoverNode(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent(out NodeActionChild child))
        {
            if (s_currentNode is not null)
            {
                OnChangeSelectedNode?.Invoke(null);
                Clear();
            }
            return;
        }

        Vector3 playerPos = m_player.position;
        Vector3 hitPos = hit.collider.transform.position;

        playerPos.y = 0;
        hitPos.y = 0;

        float distance = Vector3.Distance(playerPos, hitPos);

        Node node = child.Root;
        Outline outline = node.Outline;

        if (distance > node.GetInteractionRange())
        {
            Clear();
            return;
        }

        if (node != s_currentNode || outline != s_currentOutline)
        {
            Clear();

            s_currentNode = node;
            s_currentOutline = outline;

            if (s_currentOutline)
                s_currentOutline.enabled = true;

            OnChangeSelectedNode?.Invoke(s_currentNode);
        }
    }

    private void UpdateHoverTile(RaycastHit hit)
    {
        Vector2Int? newTile = null;
        if (!hit.collider.CompareTag("Border"))
        {
            newTile = WorldGrid.Instance.WorldToGrid(hit.point);
        }
        if (newTile != s_currentTile)
        {
            s_currentTile = newTile;
            OnChangeSelectedTile?.Invoke(s_currentTile);
        }
    }

    private void OnInteract(InputValue value)
    {
        s_currentNode?.Interact();
    }

    private void Clear()
    {
        if (s_currentOutline)
            s_currentOutline.enabled = false;

        s_currentNode = null;
        s_currentOutline = null;
        s_currentTile = null;
    }

    private void OnPoint(InputValue value)
    {
        s_mousePos = value.Get<Vector2>();
    }
}