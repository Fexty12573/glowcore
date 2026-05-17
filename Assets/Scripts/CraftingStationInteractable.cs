using GlowCore.UI.Inventory;
using ScriptableObjects;
using UnityEngine;

public class CraftingStationInteractable : MonoBehaviour, IInteractable
{
    // Instance Fields
    [SerializeField] private float m_closeDistance = 5f;
    [SerializeField] private CraftingStation m_craftingStation;
    private CraftingStationUI m_craftingStationUI;
    private PlayerInventory m_playerInventory;
    private bool m_isOpen;

    // Public Methods
    public void Interact()
    {
        SetCraftingStationOpen(!m_isOpen);
    }

    public string GetActionPromptText() => "Craft";

    // Private Methods
    private void Start()
    {
        m_craftingStationUI = FindFirstObjectByType<CraftingStationUI>(FindObjectsInactive.Include);
        if (m_craftingStationUI == null)
        {
            Debug.LogError("CraftingStationInteractable: Could not find CraftingStationUI in scene.");
            return;
        }

        m_playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (m_playerInventory == null)
        {
            Debug.LogError("CraftingStationInteractable: Could not find Inventory in scene.");
            return;
        }

        m_playerInventory.OnInventoryToggled += OnInventoryToggled;
        m_playerInventory.OnCloseUIRequested += OnCloseUIRequested;
        m_craftingStationUI.OnCloseRequested += OnCloseRequested;
        m_craftingStationUI.Hide();
    }

    private void OnDestroy()
    {
        if (m_playerInventory != null)
        {
            m_playerInventory.OnInventoryToggled -= OnInventoryToggled;
            m_playerInventory.OnCloseUIRequested -= OnCloseUIRequested;
        }

        if (m_craftingStationUI != null)
            m_craftingStationUI.OnCloseRequested -= OnCloseRequested;
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
            SetCraftingStationOpen(false);
    }

    private void OnInventoryToggled(bool isOpen)
    {
        if (isOpen && m_isOpen)
            SetCraftingStationOpen(false);
    }

    private void OnCloseUIRequested()
    {
        if (m_isOpen)
            SetCraftingStationOpen(false);
    }

    private void OnCloseRequested()
    {
        SetCraftingStationOpen(false);
    }

    private void SetCraftingStationOpen(bool open)
    {
        m_isOpen = open;
        m_playerInventory?.SetCraftingStationOpen(open);
        m_craftingStationUI?.SetCraftingStation(m_craftingStation);
        m_craftingStationUI?.SetVisible(open);
    }
}
