using ScriptableObjects;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI m_cursorCountText;
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

        private ItemSlotUI m_heldSlot;
        private IItemContainer m_heldContainer;
        private int m_heldSlotIndex = -1;
        private bool m_isHolding;

        // Floating-hold state. Split from a slot via right-click. Independent of any source slot's
        // live data: the source slot was already mutated when the split happened. m_floatingSource
        // is only used to put the items back if the player closes the UI without placing them.
        private Item m_floatingItem;
        private int m_floatingAmount;
        private IItemContainer m_floatingSource;

        private ItemSlotUI m_hoveredSlot;

        public bool IsHoldingItem => m_isHolding || m_floatingAmount > 0;

        private void Awake()
        {
            if (m_cursorIcon != null)
            {
                m_cursorIcon.gameObject.SetActive(true);
                m_cursorIcon.enabled = false;
                m_cursorIcon.raycastTarget = false;
            }
        }

        // Right-click halve doesn't go through IDragHandler, so there's no pointer-move event to ride
        // for cursor following or "release outside any slot" detection. Polled only while
        // floating-holding, no cost when idle.
        private void Update()
        {
            if (m_floatingAmount <= 0)
                return;

            UpdateCursorPosition();
            HandleOffSlotClick();
        }

        // When floating-holding, a click that lands anywhere except a slot is treated as a
        // world-drop intent (matching the existing left-drag release-outside behavior). Slot
        // clicks are dispatched by IPointerDownHandler before this runs, so m_hoveredSlot is the
        // authoritative "did the click hit a slot" signal.
        private void HandleOffSlotClick()
        {
            if (Mouse.current == null)
                return;

            var leftPressed = Mouse.current.leftButton.wasPressedThisFrame;
            var rightPressed = Mouse.current.rightButton.wasPressedThisFrame;
            if (!leftPressed && !rightPressed)
                return;

            if (m_hoveredSlot != null)
                return;

            // Chest / GlowCore source items don't drop to the world. Falling back to cancel keeps
            // the items recoverable from wherever they were split.
            if (m_floatingSource is not PlayerInventory player || player.IsGlowCoreUIOpen)
            {
                CancelHeldItem();
                return;
            }

            var dropAmount = rightPressed ? 1 : m_floatingAmount;
            var dropPos = m_dropPoint != null ? m_dropPoint.position : player.transform.position;
            ItemStackDrop.Spawn(new ItemStack(m_floatingItem, dropAmount), dropPos);

            m_floatingAmount -= dropAmount;
            if (m_floatingAmount <= 0)
                EndFloatingHold();
            else
                UpdateCursorCount(m_floatingAmount);
        }

        private void Start()
        {
            m_inventoryService = FindFirstObjectByType<PlayerInventory>();
            if (m_inventoryService == null)
            {
                Debug.LogError("InventoryUI: Could not find Inventory in scene.");
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
            var container = (IItemContainer)m_inventoryService;

            m_allSlots = new ItemSlotUI[totalSlots];

            for (var i = 0; i < hotbarSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_hotbarRowParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, container, i, m_inventoryService.GetSlotData(i));
                slotUI.SetHotbarStyle(i + 1);
                m_allSlots[i] = slotUI;
            }

            for (var i = hotbarSlots; i < totalSlots; i++)
            {
                var slotGo = Instantiate(m_slotPrefab, m_gridParent);
                var slotUI = slotGo.GetComponent<ItemSlotUI>();
                slotUI.Initialize(this, container, i, m_inventoryService.GetSlotData(i));
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
                m_craftingUI.Show();
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

        public void OnSlotPressed(ItemSlotUI slot)
        {
            if (m_isHolding || slot == null || slot.Container == null)
                return;

            if (m_floatingAmount > 0)
            {
                TryDumpFloatingOnSlot(slot);
                return;
            }

            var data = slot.Container.GetSlotData(slot.SlotIndex);
            if (data.IsValid)
                PickUpItem(slot, data);
        }

        public void OnSlotRightClicked(ItemSlotUI slot)
        {
            if (slot == null || slot.Container == null || m_isHolding)
                return;

            if (m_floatingAmount > 0)
                TryPlaceOneOnSlot(slot);
            else
                TryHalveSlot(slot);
        }

        private void TryHalveSlot(ItemSlotUI slot)
        {
            var data = slot.Container.GetSlotData(slot.SlotIndex);
            // Slots holding a single item aren't splittable, halving would just be a pickup, which
            // the left-click drag already handles. Skipping here keeps unstackable items (axe etc.)
            // from being lifted by an accidental right-click.
            if (!data.IsValid || data.Amount <= 1)
                return;

            var pickup = data.Amount / 2;
            slot.Container.SetSlot(slot.SlotIndex, data.Item, data.Amount - pickup);
            BeginFloatingHold(data.Item, pickup, slot.Container);
        }

        private void TryPlaceOneOnSlot(ItemSlotUI slot)
        {
            var data = slot.Container.GetSlotData(slot.SlotIndex);

            if (data.IsValid)
            {
                if (data.Item != m_floatingItem || data.Amount >= m_floatingItem.MaxStack)
                    return;
                slot.Container.SetSlot(slot.SlotIndex, m_floatingItem, data.Amount + 1);
            }
            else
            {
                slot.Container.SetSlot(slot.SlotIndex, m_floatingItem, 1);
            }

            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.UIDrag, AudioManager.AudioChannel.Player);
            m_floatingAmount--;
            if (m_floatingAmount <= 0)
                EndFloatingHold();
            else
                UpdateCursorCount(m_floatingAmount);
        }

        private void TryDumpFloatingOnSlot(ItemSlotUI slot)
        {
            var data = slot.Container.GetSlotData(slot.SlotIndex);

            if (!data.IsValid)
            {
                slot.Container.SetSlot(slot.SlotIndex, m_floatingItem, m_floatingAmount);
                EndFloatingHold();
                return;
            }

            if (data.Item == m_floatingItem)
            {
                var space = m_floatingItem.MaxStack - data.Amount;
                if (space <= 0)
                    return;
                var move = Mathf.Min(space, m_floatingAmount);
                slot.Container.SetSlot(slot.SlotIndex, m_floatingItem, data.Amount + move);
                m_floatingAmount -= move;
                if (m_floatingAmount <= 0)
                    EndFloatingHold();
                else
                    UpdateCursorCount(m_floatingAmount);
                return;
            }

            // Different item, Minecraft-style swap: place floating into target, target's old
            // contents become the new floating stack. Source slot of the original split is left
            // alone; on UI close any unplaced floating goes back to wherever it was split from.
            slot.Container.SetSlot(slot.SlotIndex, m_floatingItem, m_floatingAmount);
            m_floatingItem = data.Item;
            m_floatingAmount = data.Amount;
            UpdateCursorIcon(m_floatingItem);
            UpdateCursorCount(m_floatingAmount);
        }

        private void BeginFloatingHold(Item item, int amount, IItemContainer source)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.UIDrag, AudioManager.AudioChannel.Player);
            m_floatingItem = item;
            m_floatingAmount = amount;
            m_floatingSource = source;

            UpdateCursorIcon(item);
            UpdateCursorCount(amount);

            if (m_tooltip != null)
                m_tooltip.Hide();

            UpdateCursorPosition();
        }

        private void EndFloatingHold()
        {
            m_floatingItem = null;
            m_floatingAmount = 0;
            m_floatingSource = null;

            HideCursor();
        }

        private void UpdateCursorIcon(Item item)
        {
            if (m_cursorIcon == null || item == null || item.Icon == null)
                return;
            m_cursorIcon.texture = item.Icon.texture;
            m_cursorIcon.color = new Color(1f, 1f, 1f, 0.75f);
            m_cursorIcon.enabled = true;
            m_cursorIcon.transform.SetAsLastSibling();
        }

        private void UpdateCursorCount(int amount)
        {
            if (m_cursorCountText == null)
                return;
            var show = amount > 1;
            m_cursorCountText.enabled = show;
            if (show)
                m_cursorCountText.text = amount.ToString();
        }

        private void HideCursor()
        {
            if (m_cursorIcon != null)
            {
                m_cursorIcon.enabled = false;
                m_cursorIcon.color = Color.white;
            }
            if (m_cursorCountText != null)
                m_cursorCountText.enabled = false;
        }

        private void ReturnFloatingToSource()
        {
            if (m_floatingAmount <= 0 || m_floatingItem == null || m_floatingSource == null)
                return;
            m_floatingSource.AddStack(m_floatingItem, m_floatingAmount);
        }

        public void OnSlotHoverEnter(ItemSlotUI slot)
        {
            m_hoveredSlot = slot;

            if (!m_isHolding && m_floatingAmount <= 0 && m_tooltip != null && slot.Container != null)
            {
                var data = slot.Container.GetSlotData(slot.SlotIndex);
                if (data.IsValid)
                    m_tooltip.Show(data.Item);
            }
        }

        public void OnSlotHoverExit(ItemSlotUI slot)
        {
            if (m_hoveredSlot == slot)
                m_hoveredSlot = null;

            if (m_tooltip != null)
                m_tooltip.Hide();
        }

        private void PickUpItem(ItemSlotUI slot, SlotData data)
        {
            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.UIDrag, AudioManager.AudioChannel.Player);
            m_heldSlot = slot;
            m_heldContainer = slot.Container;
            m_heldSlotIndex = slot.SlotIndex;
            m_isHolding = true;

            slot.SetGhosted(true);

            UpdateCursorIcon(data.Item);
            UpdateCursorCount(data.Amount);

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

            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.UIDrop, AudioManager.AudioChannel.Player);

            var hovered = m_hoveredSlot;
            var heldData = m_heldContainer.GetSlotData(m_heldSlotIndex);

            if (hovered != null && hovered != m_heldSlot && hovered.Container != null)
            {
                var hoverData = hovered.Container.GetSlotData(hovered.SlotIndex);

                if (hovered.Container == m_heldContainer)
                {
                    if (hoverData.IsValid && heldData.IsValid && heldData.Item == hoverData.Item)
                        m_heldContainer.TryMerge(m_heldSlotIndex, hovered.SlotIndex);
                    else
                        m_heldContainer.Swap(m_heldSlotIndex, hovered.SlotIndex);
                }
                else
                {
                    ContainerOps.MoveStack(m_heldContainer, m_heldSlotIndex, heldData,
                                           hovered.Container, hovered.SlotIndex, hoverData);
                }
            }
            else if (hovered == null)
            {
                // Don't drop to world while the GlowCore UI is open — the player is trying to feed
                // the core, not litter the floor. CancelHeldItem snaps the stack back to its slot.
                if (m_heldContainer is PlayerInventory player && !player.IsGlowCoreUIOpen)
                    DropItemToWorld(player);
            }

            CancelHeldItem();
        }

        private void DropItemToWorld(PlayerInventory player)
        {
            var dropPos = m_dropPoint != null ? m_dropPoint.position : player.transform.position;
            player.DropItem(m_heldSlotIndex, dropPos);
        }

        public void CancelHeldItem()
        {
            var previousHeld = m_heldSlot;

            m_heldSlot = null;
            m_heldContainer = null;
            m_heldSlotIndex = -1;
            m_isHolding = false;

            ReturnFloatingToSource();
            EndFloatingHold();

            HideCursor();

            if (previousHeld != null)
                previousHeld.SetGhosted(false);
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
