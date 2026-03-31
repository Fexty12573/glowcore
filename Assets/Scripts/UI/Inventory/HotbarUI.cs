using UnityEngine;

namespace GlowCore.UI.Inventory
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory m_playerInventory;
        [SerializeField] private Transform m_slotParent;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private CanvasGroup m_canvasGroup;

        private const float kDimmedAlpha = 0.35f;

        private IInventoryService m_service;
        private HotbarSlotUI[] m_slots;

        private void Start()
        {
            m_service = m_playerInventory;
            BuildSlots();
            m_service.OnHotbarSelectionChanged += OnSelectionChanged;
            m_service.OnSlotChanged += OnSlotDataChanged;
            m_service.OnInventoryToggled += OnInventoryToggled;
            UpdateSelection();
        }

        private void OnDestroy()
        {
            if (m_service != null)
            {
                m_service.OnHotbarSelectionChanged -= OnSelectionChanged;
                m_service.OnSlotChanged -= OnSlotDataChanged;
                m_service.OnInventoryToggled -= OnInventoryToggled;
            }
        }

        private void BuildSlots()
        {
            m_slots = new HotbarSlotUI[m_service.HotbarSlotCount];
            for (var i = 0; i < m_service.HotbarSlotCount; i++)
            {
                var go = Instantiate(m_slotPrefab, m_slotParent);
                var slot = go.GetComponent<HotbarSlotUI>();
                slot.Initialize(m_service, i);
                m_slots[i] = slot;
            }
        }

        private void OnSelectionChanged(int index)
        {
            UpdateSelection();
        }

        private void OnSlotDataChanged(SlotChangedEvent e)
        {
            if (m_service.IsHotbarSlot(e.SlotIndex))
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
                m_slots[i].SetSelected(i == m_service.SelectedHotbarIndex);
        }

        public void RefreshAll()
        {
            if (m_slots == null)
                return;
            for (var i = 0; i < m_slots.Length; i++)
                m_slots[i].Refresh(m_service.GetSlotData(i));
        }
    }
}
