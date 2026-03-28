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

    private static Node m_currentNode;
    private static Outline m_currentOutline;
    private static Vector2Int? m_currentTile;
    private static Vector2 m_mousePos;

    // both can also be called with null to notify that no Node is hovered over.
    public static event Action<Node> OnChangeSelectedNode;
    public static event Action<Vector2Int?> OnChangeSelectedTile;

    public static void OnHandItemChange()
    {
        OnChangeSelectedNode?.Invoke(m_currentNode);
        OnChangeSelectedTile?.Invoke(m_currentTile);
    }
    private void Update()
    {
        Ray ray = m_camera.ScreenPointToRay(m_mousePos);
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
            if (m_currentNode is not null)
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

        if (node != m_currentNode || outline != m_currentOutline)
        {
            Clear();

            m_currentNode = node;
            m_currentOutline = outline;

            if (m_currentOutline)
                m_currentOutline.enabled = true;

            OnChangeSelectedNode?.Invoke(m_currentNode);
        }
    }

    private void UpdateHoverTile(RaycastHit hit)
    {
        Vector2Int? newTile = null;
        if (!hit.collider.CompareTag("Border"))
        {
            newTile = WorldGrid.Instance.WorldToGrid(hit.point);
        }
        if (newTile != m_currentTile)
        {
            m_currentTile = newTile;
            OnChangeSelectedTile?.Invoke(m_currentTile);
        }
    }

    private void OnInteract(InputValue value)
    {
        m_currentNode?.Interact();
    }

    private void Clear()
    {
        if (m_currentOutline)
            m_currentOutline.enabled = false;

        m_currentNode = null;
        m_currentOutline = null;
        m_currentTile = null;
    }

    private void OnPoint(InputValue value)
    {
        m_mousePos = value.Get<Vector2>();
    }
}