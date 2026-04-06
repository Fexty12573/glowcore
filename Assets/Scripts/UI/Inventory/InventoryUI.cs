using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInventory m_playerInventory;
        [SerializeField] private Transform m_gridParent;
        [SerializeField] private Transform m_hotbarRowParent;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private CanvasGroup m_panelCanvasGroup;

        [Header("Cursor Item")]
        [SerializeField] private RawImage m_cursorIcon;
        [SerializeField] private Canvas m_parentCanvas;

        [Header("Backdrop")]
        [SerializeField] private CanvasGroup m_backdropCanvasGroup;

        [Header("World Drop")]
        [SerializeField] private Transform m_dropPoint;

        [Header("Tooltip")]
        [SerializeField] private TooltipUI m_tooltip;

        [Header("Crafting")]
        [SerializeField] private CraftingUI m_craftingUI;

        private IInventoryService m_service;
        private ItemSlotUI[] m_allSlots;
        private int m_heldSlotIndex = -1;
        private bool m_isHolding;
        private int m_hoveredSlotIndex = -1;

        public bool IsHoldingItem => m_isHolding;
        public int HeldSlotIndex => m_heldSlotIndex;

        private void Awake()
        {
            if (m_cursorIcon != null)
            {
                m_cursorIcon.gameObject.SetActive(true);
                m_cursorIcon.enabled = false;
                m_cursorIcon.raycastTarget = false;
            }
        }

        private void Start()
        {
            m_service = m_playerInventory;
            BuildGrid();
            m_service.OnSlotChanged += OnSlotDataChanged;
            m_service.OnInventoryToggled += OnInventoryToggled;

            SetPanelVisible(m_service.IsOpen);
        }

        private void OnDestroy()
        {
            if (m_service != null)
            {
                m_service.OnSlotChanged -= OnSlotDataChanged;
                m_service.OnInventoryToggled -= OnInventoryToggled;
            }
        }

        private void BuildGrid()
        {
            var totalSlots = m_service.SlotCount;
            var hotbarSlots = m_service.HotbarSlotCount;

            m_allSlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_hotbarRowParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i, m_service.GetSlotData(i));
                slotUI.SetHotbarStyle(i + 1);
                m_allSlots[i] = slotUI;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_gridParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i, m_service.GetSlotData(i));
                m_allSlots[i] = slotUI;
            }
        }

        private void OnSlotDataChanged(SlotChangedEvent evt)
        {
            if (evt.SlotIndex >= 0 && evt.SlotIndex < m_allSlots.Length)
                m_allSlots[evt.SlotIndex].Refresh(evt.Data);
        }

        private void OnInventoryToggled(bool isOpen)
        {
            SetPanelVisible(isOpen);

            if (!isOpen)
            {
                CancelHeldItem();
                if (m_craftingUI != null)
                    m_craftingUI.Hide();
            }
        }

        private void SetPanelVisible(bool visible)
        {
            m_panelCanvasGroup?.SetVisible(visible);

            if (m_backdropCanvasGroup != null)
                m_backdropCanvasGroup.alpha = visible ? 1f : 0f;
        }

        public void OnSlotPressed(int slotIndex)
        {
            if (m_isHolding)
                return;

            var data = m_service.GetSlotData(slotIndex);
            if (data.IsValid)
                PickUpItem(slotIndex, data);
        }

        public void OnSlotHoverEnter(ItemSlotUI slot)
        {
            m_hoveredSlotIndex = slot.SlotIndex;

            if (!m_isHolding && m_tooltip != null)
            {
                var data = m_service.GetSlotData(slot.SlotIndex);
                if (data.IsValid)
                    m_tooltip.Show(data.Item);
            }
        }

        public void OnSlotHoverExit(ItemSlotUI slot)
        {
            if (m_hoveredSlotIndex == slot.SlotIndex)
                m_hoveredSlotIndex = -1;

            if (m_tooltip != null)
                m_tooltip.Hide();
        }

        private void PickUpItem(int slotIndex, SlotData data)
        {
            m_heldSlotIndex = slotIndex;
            m_isHolding = true;

            SetSlotGhosted(slotIndex, true);

            if (m_cursorIcon != null && data.Icon != null)
            {
                m_cursorIcon.texture = data.Icon.texture;
                m_cursorIcon.color = new Color(1f, 1f, 1f, 0.75f);
                m_cursorIcon.enabled = true;
                m_cursorIcon.transform.SetAsLastSibling();
            }

            if (m_tooltip != null)
                m_tooltip.Hide();

            UpdateCursorPosition();
        }

        public void OnDragUpdate()
        {
            if (m_isHolding)
                UpdateCursorPosition();

            if (m_tooltip != null)
                m_tooltip.UpdatePosition();
        }

        public void OnSlotReleased()
        {
            if (m_isHolding)
                DropHeldItem();
        }

        private void DropHeldItem()
        {
            if (!m_isHolding)
                return;

            if (m_hoveredSlotIndex >= 0 && m_hoveredSlotIndex != m_heldSlotIndex)
            {
                var targetData = m_service.GetSlotData(m_hoveredSlotIndex);
                var heldData = m_service.GetSlotData(m_heldSlotIndex);

                if (targetData.IsValid && heldData.IsValid && heldData.Item == targetData.Item)
                    m_service.TryMerge(m_heldSlotIndex, m_hoveredSlotIndex);
                else
                    m_service.Swap(m_heldSlotIndex, m_hoveredSlotIndex);
            }
            else if (m_hoveredSlotIndex == -1)
            {
                DropItemToWorld();
            }

            CancelHeldItem();
        }

        private void DropItemToWorld()
        {
            var dropPos = m_dropPoint != null ? m_dropPoint.position : m_playerInventory.transform.position;
            m_service.DropItem(m_heldSlotIndex, dropPos);
        }

        private void CancelHeldItem()
        {
            var previousHeld = m_heldSlotIndex;

            m_heldSlotIndex = -1;
            m_isHolding = false;

            if (m_cursorIcon != null)
            {
                m_cursorIcon.enabled = false;
                m_cursorIcon.color = Color.white;
            }

            SetSlotGhosted(previousHeld, false);
        }

        private void SetSlotGhosted(int flatIndex, bool ghosted)
        {
            if (flatIndex < 0 || flatIndex >= m_allSlots.Length)
                return;
            m_allSlots[flatIndex].SetGhosted(ghosted);
        }

        private void UpdateCursorPosition()
        {
            if (m_cursorIcon == null || m_parentCanvas == null)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_parentCanvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                m_parentCanvas.worldCamera,
                out var localPoint);

            m_cursorIcon.rectTransform.localPosition = localPoint;
        }

        public void RefreshAll()
        {
            if (m_allSlots == null)
                return;
            for (var i = 0; i < m_allSlots.Length; i++)
                m_allSlots[i].Refresh(m_service.GetSlotData(i));
        }
    }
}
