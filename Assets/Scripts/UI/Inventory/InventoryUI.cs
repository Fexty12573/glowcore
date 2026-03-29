using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    /// <summary>Manages the inventory panel UI. Hold left mouse to drag items, release to drop.</summary>
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

        private ItemSlotUI[] m_allSlots;
        private int m_heldSlotIndex = -1;
        private bool m_isHolding;
        private int m_hoveredSlotIndex = -1;

        public PlayerInventory PlayerInventory => m_playerInventory;
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
            BuildGrid();
            m_playerInventory.Inventory.OnSlotChanged += OnSlotDataChanged;
            m_playerInventory.OnInventoryToggled += OnInventoryToggled;

            SetPanelVisible(m_playerInventory.IsOpen);
        }

        private void OnDestroy()
        {
            if (m_playerInventory != null && m_playerInventory.Inventory != null)
                m_playerInventory.Inventory.OnSlotChanged -= OnSlotDataChanged;

            if (m_playerInventory != null)
                m_playerInventory.OnInventoryToggled -= OnInventoryToggled;
        }

        private void Update()
        {
            if (m_isHolding)
            {
                UpdateCursorPosition();

                if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
                    DropHeldItem();
            }
        }

        private void BuildGrid()
        {
            var inv = m_playerInventory.Inventory;
            var totalSlots = inv.Size;
            var hotbarSlots = m_playerInventory.HotbarSlots;

            m_allSlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_hotbarRowParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i);
                slotUI.SetHotbarStyle(i + 1);
                m_allSlots[i] = slotUI;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_gridParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, i);
                m_allSlots[i] = slotUI;
            }
        }

        private void OnSlotDataChanged(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < m_allSlots.Length)
                m_allSlots[slotIndex].Refresh();
        }

        private void OnInventoryToggled(bool isOpen)
        {
            SetPanelVisible(isOpen);

            if (!isOpen)
            {
                CancelHeldItem();
                if (m_craftingUI != null) m_craftingUI.Hide();
            }
        }

        private void SetPanelVisible(bool visible)
        {
            if (m_panelCanvasGroup != null)
            {
                m_panelCanvasGroup.alpha = visible ? 1f : 0f;
                m_panelCanvasGroup.interactable = visible;
                m_panelCanvasGroup.blocksRaycasts = visible;
            }

            if (m_backdropCanvasGroup != null)
                m_backdropCanvasGroup.alpha = visible ? 1f : 0f;
        }

        /// <summary>Called by ItemSlotUI on pointer down. Starts dragging if the slot has an item.</summary>
        public void OnSlotPressed(int slotIndex)
        {
            if (m_isHolding) return;

            var stack = m_playerInventory.Inventory.GetSlot(slotIndex);
            if (stack != null && stack.Valid)
                PickUpItem(slotIndex);
        }

        /// <summary>Called by ItemSlotUI on hover enter.</summary>
        public void OnSlotHoverEnter(ItemSlotUI slot)
        {
            m_hoveredSlotIndex = slot.SlotIndex;

            if (!m_isHolding && m_tooltip != null)
            {
                var stack = slot.GetItemStack();
                if (stack != null)
                    m_tooltip.Show(stack);
            }
        }

        /// <summary>Called by ItemSlotUI on hover exit.</summary>
        public void OnSlotHoverExit(ItemSlotUI slot)
        {
            if (m_hoveredSlotIndex == slot.SlotIndex)
                m_hoveredSlotIndex = -1;

            if (m_tooltip != null)
                m_tooltip.Hide();
        }

        private void PickUpItem(int slotIndex)
        {
            m_heldSlotIndex = slotIndex;
            m_isHolding = true;

            SetSlotGhosted(slotIndex, true);

            var stack = m_playerInventory.Inventory.GetSlot(slotIndex);
            if (m_cursorIcon != null && stack != null && stack.Item.Icon != null)
            {
                m_cursorIcon.texture = stack.Item.Icon;
                m_cursorIcon.color = new Color(1f, 1f, 1f, 0.75f);
                m_cursorIcon.enabled = true;
                m_cursorIcon.transform.SetAsLastSibling();
            }

            if (m_tooltip != null)
                m_tooltip.Hide();

            UpdateCursorPosition();
        }

        private void DropHeldItem()
        {
            if (!m_isHolding) return;

            var inv = m_playerInventory.Inventory;

            if (m_hoveredSlotIndex >= 0 && m_hoveredSlotIndex != m_heldSlotIndex)
            {
                var targetStack = inv.GetSlot(m_hoveredSlotIndex);
                var heldStack = inv.GetSlot(m_heldSlotIndex);

                if (targetStack != null && targetStack.Valid && heldStack != null &&
                    heldStack.Item == targetStack.Item)
                {
                    inv.TryMerge(m_heldSlotIndex, m_hoveredSlotIndex);
                }
                else
                {
                    inv.Swap(m_heldSlotIndex, m_hoveredSlotIndex);
                }
            }
            else if (m_hoveredSlotIndex == -1)
            {
                DropItemToWorld();
            }

            CancelHeldItem();
        }

        private void DropItemToWorld()
        {
            var inv = m_playerInventory.Inventory;
            var stack = inv.GetSlot(m_heldSlotIndex);
            if (stack == null || !stack.Valid) return;

            var dropPos = m_dropPoint != null ? m_dropPoint.position : m_playerInventory.transform.position;
            ItemStackDrop.Spawn(stack, dropPos);
            stack.Set(null, 0);
            inv.NotifySlotChanged(m_heldSlotIndex);
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
            if (flatIndex < 0 || flatIndex >= m_allSlots.Length) return;
            m_allSlots[flatIndex].SetGhosted(ghosted);
        }

        private void UpdateCursorPosition()
        {
            if (m_cursorIcon == null || m_parentCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_parentCanvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                m_parentCanvas.worldCamera,
                out var localPoint);

            m_cursorIcon.rectTransform.localPosition = localPoint;
        }

        /// <summary>Refresh all slots (upper grid + hotbar row).</summary>
        public void RefreshAll()
        {
            if (m_allSlots == null) return;
            for (var i = 0; i < m_allSlots.Length; i++)
                m_allSlots[i].Refresh();
        }
    }
}
