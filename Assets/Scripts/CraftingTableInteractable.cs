using GlowCore.UI.Inventory;
using GlowCore.World;
using UnityEngine;

public class CraftingTableInteractable : MonoBehaviour, IInteractable
{
    // Instance Fields
    private CraftingTableUI m_craftingTableUI;
    private PlayerInventory m_playerInventory;
    private float m_closeDistance;
    private bool m_isOpen;

    // Public Methods
    public void Interact()
    {
        SetCraftingTableOpen(!m_isOpen);
    }

    // Private Methods
    private void Start()
    {
        m_craftingTableUI = FindFirstObjectByType<CraftingTableUI>(FindObjectsInactive.Include);
        if (m_craftingTableUI == null)
        {
            Debug.LogError("CraftingTableInteractable: Could not find CraftingTableUI in scene.");
            return;
        }

        m_playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (m_playerInventory == null)
        {
            Debug.LogError("CraftingTableInteractable: Could not find PlayerInventory in scene.");
            return;
        }

        var node = GetComponent<Node>();
        m_closeDistance = node != null ? node.GetInteractionRange() * 1f : 5f;

        m_playerInventory.OnInventoryToggled += OnInventoryToggled;
        m_playerInventory.OnCloseUIRequested += OnCloseUIRequested;
        m_craftingTableUI.OnCloseRequested += OnCloseRequested;
        m_craftingTableUI.Hide();
    }

    private void OnDestroy()
    {
        if (m_playerInventory != null)
        {
            m_playerInventory.OnInventoryToggled -= OnInventoryToggled;
            m_playerInventory.OnCloseUIRequested -= OnCloseUIRequested;
        }

        if (m_craftingTableUI != null)
            m_craftingTableUI.OnCloseRequested -= OnCloseRequested;
    }

    private void Update()
    {
        if (!m_isOpen)
            return;

        var playerPos = m_playerInventory.transform.position;
        var tablePos = transform.position;
        playerPos.y = 0f;
        tablePos.y = 0f;

        if (Vector3.Distance(playerPos, tablePos) > m_closeDistance)
            SetCraftingTableOpen(false);
    }

    private void OnInventoryToggled(bool isOpen)
    {
        if (isOpen && m_isOpen)
            SetCraftingTableOpen(false);
    }

    private void OnCloseUIRequested()
    {
        if (m_isOpen)
            SetCraftingTableOpen(false);
    }

    private void OnCloseRequested()
    {
        SetCraftingTableOpen(false);
    }

    private void SetCraftingTableOpen(bool open)
    {
        m_isOpen = open;
        m_playerInventory?.SetCraftingTableOpen(open);
        m_craftingTableUI?.SetVisible(open);
    }
}
