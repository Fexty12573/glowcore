using GlowCore.UI.Inventory;
using UnityEngine;

[RequireComponent(typeof(Chest))]
public class ChestInteractable : MonoBehaviour, IInteractable
{
    // Instance Fields
    [SerializeField] private float m_closeDistance = 5f;

    private Chest m_chest;
    private ChestUI m_chestUI;
    private PlayerInventory m_playerInventory;
    private bool m_isOpen;

    // Public Methods
    public void Interact()
    {
        SetChestOpen(!m_isOpen);
    }

    public string GetActionPromptText() => "Open Chest";

    // Private Methods
    private void Awake()
    {
        m_chest = GetComponent<Chest>();
    }

    private void Start()
    {
        m_chestUI = ChestUI.Instance;
        if (m_chestUI == null)
        {
            Debug.LogWarning("ChestInteractable: Could not find ChestUI Singleton d.");
            return;
        }

        m_playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (m_playerInventory == null)
        {
            Debug.LogWarning("ChestInteractable: Could not find PlayerInventory in scene.");
            return;
        }

        m_playerInventory.OnInventoryToggled += OnInventoryToggled;
        m_playerInventory.OnCloseUIRequested += OnCloseUIRequested;
        m_chestUI.OnCloseRequested += OnCloseRequested;
    }

    private void OnDestroy()
    {
        if (m_playerInventory != null)
        {
            m_playerInventory.OnInventoryToggled -= OnInventoryToggled;
            m_playerInventory.OnCloseUIRequested -= OnCloseUIRequested;
        }

        if (m_chestUI != null)
            m_chestUI.OnCloseRequested -= OnCloseRequested;
    }

    private void Update()
    {
        if (!m_isOpen)
            return;

        var playerPos = m_playerInventory.transform.position;
        var chestPos = transform.position;
        playerPos.y = 0f;
        chestPos.y = 0f;

        if (Vector3.Distance(playerPos, chestPos) > m_closeDistance)
            SetChestOpen(false);
    }

    private void OnInventoryToggled(bool isOpen)
    {
        if (isOpen && m_isOpen)
            SetChestOpen(false);
    }

    private void OnCloseUIRequested()
    {
        if (m_isOpen)
            SetChestOpen(false);
    }

    private void OnCloseRequested()
    {
        SetChestOpen(false);
    }

    private void SetChestOpen(bool open)
    {
        m_isOpen = open;
        m_playerInventory?.SetChestOpen(open);

        if (m_chestUI == null)
            return;

        if (open)
            m_chestUI.Show(m_chest);
        else
            m_chestUI.Hide();
    }
}
