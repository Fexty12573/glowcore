using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
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

        [Header("Close Button")]
        [SerializeField] private Button m_closeButton;

        private IInventoryService m_inventoryService;
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
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("InventoryUI: Could not find PlayerInventory in scene.");
                return;
            }

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(OnCloseButtonClicked);

            BuildGrid();
            m_inventoryService.OnSlotChanged += OnSlotDataChanged;
            m_inventoryService.OnInventoryToggled += OnInventoryToggled;

            SetPanelVisible(m_inventoryService.IsOpen);
        }

        private void OnDestroy()
        {
            if (m_closeButton != null)
                m_closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            if (m_inventoryService != null)
            {
                m_inventoryService.OnSlotChanged -= OnSlotDataChanged;
                m_inventoryService.OnInventoryToggled -= OnInventoryToggled;
            }
        }

        private void OnCloseButtonClicked() => m_inventoryService?.SetInventoryOpen(false);

        private void BuildGrid()
        {
            var totalSlots = m_inventoryService.SlotCount;
            var hotbarSlots = m_inventoryService.HotbarSlotCount;

            m_allSlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_hotbarRowParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i, m_inventoryService.GetSlotData(i));
                slotUI.SetHotbarStyle(i + 1);
                m_allSlots[i] = slotUI;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_gridParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i, m_inventoryService.GetSlotData(i));
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
            else
                m_craftingUI.Show(); // Makes that the Crafting UI is opened by default
        }

        private void SetPanelVisible(bool visible)
        {
            m_panelCanvasGroup?.SetVisible(visible);

            if (m_backdropCanvasGroup != null)
            {
                m_backdropCanvasGroup.alpha = visible ? 1f : 0f;
                m_backdropCanvasGroup.blocksRaycasts = visible;
            }
        }

        public void OnSlotPressed(int slotIndex)
        {
            if (m_isHolding)
                return;

            var data = m_inventoryService.GetSlotData(slotIndex);
            if (data.IsValid)
                PickUpItem(slotIndex, data);
        }

        public void OnSlotHoverEnter(ItemSlotUI slot)
        {
            m_hoveredSlotIndex = slot.SlotIndex;

            if (!m_isHolding && m_tooltip != null)
            {
                var data = m_inventoryService.GetSlotData(slot.SlotIndex);
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
                var targetData = m_inventoryService.GetSlotData(m_hoveredSlotIndex);
                var heldData = m_inventoryService.GetSlotData(m_heldSlotIndex);

                if (targetData.IsValid && heldData.IsValid && heldData.Item == targetData.Item)
                    m_inventoryService.TryMerge(m_heldSlotIndex, m_hoveredSlotIndex);
                else
                    m_inventoryService.Swap(m_heldSlotIndex, m_hoveredSlotIndex);
            }
            else if (m_hoveredSlotIndex == -1)
            {
                DropItemToWorld();
            }

            CancelHeldItem();
        }

        private void DropItemToWorld()
        {
            var dropPos = m_dropPoint != null ? m_dropPoint.position : ((MonoBehaviour)m_inventoryService).transform.position;
            m_inventoryService.DropItem(m_heldSlotIndex, dropPos);
        }

        public void CancelHeldItem()
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
                m_allSlots[i].Refresh(m_inventoryService.GetSlotData(i));
        }
    }
}
