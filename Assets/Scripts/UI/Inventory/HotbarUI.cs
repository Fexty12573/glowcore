using UnityEngine;

namespace GlowCore.UI.Inventory
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private Transform m_slotParent;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private CanvasGroup m_canvasGroup;

        private const float kDimmedAlpha = 0.35f;

        private IInventoryService m_inventoryService;
        private HotbarSlotUI[] m_slots;

        private void Start()
        {
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("HotbarUI: Could not find PlayerInventory in scene.");
                return;
            }

            BuildSlots();
            m_inventoryService.OnHotbarSelectionChanged += OnSelectionChanged;
            m_inventoryService.OnSlotChanged += OnSlotDataChanged;
            m_inventoryService.OnInventoryToggled += OnInventoryToggled;
            m_inventoryService.OnCraftingTableToggled += OnInventoryToggled;
            UpdateSelection();
        }

        private void OnDestroy()
        {
            if (m_inventoryService != null)
            {
                m_inventoryService.OnHotbarSelectionChanged -= OnSelectionChanged;
                m_inventoryService.OnSlotChanged -= OnSlotDataChanged;
                m_inventoryService.OnInventoryToggled -= OnInventoryToggled;
                m_inventoryService.OnCraftingTableToggled -= OnInventoryToggled;
            }
        }

        private void BuildSlots()
        {
            m_slots = new HotbarSlotUI[m_inventoryService.HotbarSlotCount];
            for (var i = 0; i < m_inventoryService.HotbarSlotCount; i++)
            {
                var go = Instantiate(m_slotPrefab, m_slotParent);
                var slot = go.GetComponent<HotbarSlotUI>();
                slot.Initialize(m_inventoryService, i);
                m_slots[i] = slot;
            }
        }

        private void OnSelectionChanged(int index)
        {
            UpdateSelection();
        }

        private void OnSlotDataChanged(SlotChangedEvent e)
        {
            if (m_inventoryService.IsHotbarSlot(e.SlotIndex))
            {
                m_slots[e.SlotIndex].Refresh(e.Data);
            }
        }

        private void OnInventoryToggled(bool isOpen)
        {
            SetDimmed(isOpen);
        }

        private void SetDimmed(bool dimmed)
        {
            if (m_canvasGroup == null)
                return;
            m_canvasGroup.alpha = dimmed ? kDimmedAlpha : 1f;
            m_canvasGroup.interactable = !dimmed;
            m_canvasGroup.blocksRaycasts = !dimmed;
        }

        private void UpdateSelection()
        {
            for (var i = 0; i < m_slots.Length; i++)
                m_slots[i].SetSelected(i == m_inventoryService.SelectedHotbarIndex);
        }

        public void RefreshAll()
        {
            if (m_slots == null)
                return;
            for (var i = 0; i < m_slots.Length; i++)
                m_slots[i].Refresh(m_inventoryService.GetSlotData(i));
        }
    }
}
