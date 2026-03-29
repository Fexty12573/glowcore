using UnityEngine;
using UnityEngine.InputSystem;

namespace GlowCore.UI.Inventory
{
    /// <summary>Always-visible hotbar reading from the bottom row of the inventory. Grays out when inventory is open.</summary>
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory m_playerInventory;
        [SerializeField] private Transform m_slotParent;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private CanvasGroup m_canvasGroup;

        private const float kDimmedAlpha = 0.35f;

        private HotbarSlotUI[] m_slots;

        private void Start()
        {
            BuildSlots();
            m_playerInventory.OnHotbarSelectionChanged += OnSelectionChanged;
            m_playerInventory.Inventory.OnSlotChanged += OnSlotDataChanged;
            m_playerInventory.OnInventoryToggled += OnInventoryToggled;
            UpdateSelection();
        }

        private void OnDestroy()
        {
            if (m_playerInventory != null)
            {
                m_playerInventory.OnHotbarSelectionChanged -= OnSelectionChanged;
                m_playerInventory.OnInventoryToggled -= OnInventoryToggled;
                if (m_playerInventory.Inventory != null)
                    m_playerInventory.Inventory.OnSlotChanged -= OnSlotDataChanged;
            }
        }

        private void Update()
        {
            if (Keyboard.current == null)
                return;

            for (var i = 0; i < m_playerInventory.HotbarSlots; i++)
            {
                var key = Keyboard.current[(Key)((int)Key.Digit1 + i)];
                if (key.wasPressedThisFrame)
                {
                    m_playerInventory.SelectHotbarSlot(i);
                    break;
                }
            }
        }

        private void BuildSlots()
        {
            m_slots = new HotbarSlotUI[m_playerInventory.HotbarSlots];
            for (var i = 0; i < m_playerInventory.HotbarSlots; i++)
            {
                var go = Instantiate(m_slotPrefab, m_slotParent);
                var slot = go.GetComponent<HotbarSlotUI>();
                slot.Initialize(m_playerInventory, i);
                m_slots[i] = slot;
            }
        }

        private void OnSelectionChanged(int index)
        {
            UpdateSelection();
        }

        private void OnSlotDataChanged(int flatIndex)
        {
            var hotbarIndex = m_playerInventory.InventoryToHotbarIndex(flatIndex);
            if (hotbarIndex < 0 || hotbarIndex >= m_slots.Length)
                return;
            m_slots[hotbarIndex].Refresh();
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
                m_slots[i].SetSelected(i == m_playerInventory.SelectedHotbarIndex);
        }

        /// <summary>Refresh all hotbar slot visuals.</summary>
        public void RefreshAll()
        {
            if (m_slots == null)
                return;
            for (var i = 0; i < m_slots.Length; i++)
                m_slots[i].Refresh();
        }
    }
}
