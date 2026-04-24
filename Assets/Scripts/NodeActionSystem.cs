using System;
using GlowCore.World;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
    string GetActionPromptText();
}

public class NodeActionSystem : MonoBehaviour
{
    private static NodeActionSystem s_instance;

    [SerializeField] private Camera m_camera;
    [SerializeField] private Transform m_player;
    [SerializeField] private PlayerInventory m_playerInventory;
    [SerializeField] private float m_raycastRange = 100f;
    private Node m_currentNode;
    private Outline m_currentOutline;
    private Vector2Int? m_currentTile;
    private Vector2 m_mousePos;

    public static NodeActionSystem Instance => s_instance;

    public Node CurrentNode => m_currentNode;
    public Vector2Int? CurrentTile => m_currentTile;

    // both can also be called with null to notify that nothing is hovered over.
    public event Action<Node> OnChangeSelectedNode;
    public event Action<Vector2Int?> OnChangeSelectedTile;

    public void OnHandItemChange()
    {
        OnChangeSelectedNode?.Invoke(m_currentNode);
        OnChangeSelectedTile?.Invoke(m_currentTile);
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("NodeActionSystem: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }

    private void Update()
    {
        UpdateOutlineHover();
    }

    private void OnEnable()
    {
        Node.OnNodeBroken += HandleNodeBroken;
    }

    private void OnDisable()
    {
        Node.OnNodeBroken -= HandleNodeBroken;
    }

    private void UpdateOutlineHover()
    {
        if (m_playerInventory != null && (m_playerInventory.IsOpen || m_playerInventory.IsCraftingTableOpen || m_playerInventory.IsGlowCoreUIOpen))
        {
            if (m_currentNode != null)
                OnChangeSelectedNode?.Invoke(null);
            Clear();
            return;
        }
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

        if (distance > node.GetInteractionRange() || !WorldGrid.Instance.IsNodeInBounds(node))
        {
            OnChangeSelectedNode?.Invoke(null);
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

    private void HandleNodeBroken(Node brokenNode)
    {
        if (brokenNode == m_currentNode)
        {
            OnChangeSelectedNode?.Invoke(null);
            Clear();
        }
    }

    private void OnInteract(InputValue value)
    {
        if (m_playerInventory != null && (m_playerInventory.IsOpen || m_playerInventory.IsCraftingTableOpen || m_playerInventory.IsGlowCoreUIOpen))
            return;

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