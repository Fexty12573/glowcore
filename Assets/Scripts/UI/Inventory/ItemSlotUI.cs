using GlowCore.UI;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    /// <summary>Renders a single inventory slot. Press-and-hold to drag items.</summary>
    [RequireComponent(typeof(Image))]
    public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Image m_icon;
        [SerializeField] private TextMeshProUGUI m_countText;
        [SerializeField] private Image m_border;
        [SerializeField] private TextMeshProUGUI m_keyLabel;

        private int m_slotIndex;
        private InventoryUI m_owner;
        private bool m_isHovered;
        private bool m_isHotbarSlot;
        private bool m_isGhosted;

        public int SlotIndex => m_slotIndex;

        public void Initialize(InventoryUI owner, int slotIndex)
        {
            m_owner = owner;
            m_slotIndex = slotIndex;

            if (m_keyLabel != null)
                m_keyLabel.enabled = false;

            Refresh();
        }

        /// <summary>Ghost this slot (item picked up and on cursor). Hides icon and count.</summary>
        public void SetGhosted(bool ghosted)
        {
            m_isGhosted = ghosted;
            Refresh();
        }

        /// <summary>Mark this slot as a hotbar slot with a visible key number and accent border.</summary>
        public void SetHotbarStyle(int keyNumber)
        {
            m_isHotbarSlot = true;

            if (m_keyLabel != null)
            {
                m_keyLabel.enabled = true;
                m_keyLabel.text = keyNumber.ToString();
                m_keyLabel.color = UIColors.AccentDim;
            }

            UpdateBorderColor();
        }

        public void Refresh()
        {
            if (m_owner == null) return;

            var stack = m_owner.PlayerInventory.Inventory.GetSlot(m_slotIndex);
            var hasItem = stack != null && stack.Valid;
            var showItem = hasItem && !m_isGhosted;
            var sprite = showItem ? ItemIconHelper.GetSprite(stack.Item) : null;

            if (m_icon != null)
            {
                m_icon.enabled = sprite != null;
                if (sprite != null)
                    m_icon.sprite = sprite;
            }

            if (m_countText != null)
            {
                m_countText.enabled = showItem && stack.Amount > 1;
                if (showItem && stack.Amount > 1)
                    m_countText.text = stack.Amount.ToString();
            }

            if (m_background != null)
                m_background.color = showItem ? UIColors.SlotFilledBg : UIColors.SlotBg;

            UpdateBorderColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_owner?.OnSlotPressed(m_slotIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isHovered = true;
            UpdateBorderColor();
            m_owner?.OnSlotHoverEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isHovered = false;
            UpdateBorderColor();
            m_owner?.OnSlotHoverExit(this);
        }

        private void UpdateBorderColor()
        {
            if (m_border == null) return;

            if (m_isHovered)
                m_border.color = UIColors.SlotHoverBorder;
            else if (m_isHotbarSlot)
                m_border.color = UIColors.WoodBorderLight;
            else
                m_border.color = UIColors.SlotBorder;

            if (m_background != null)
            {
                var stack = m_owner?.PlayerInventory.Inventory.GetSlot(m_slotIndex);
                var hasItem = stack != null && stack.Valid && !m_isGhosted;

                if (m_isHovered)
                    m_background.color = hasItem ? UIColors.SlotFilledBg : UIColors.SlotHoverBg;
                else
                    m_background.color = hasItem ? UIColors.SlotFilledBg : UIColors.SlotBg;
            }
        }

        /// <summary>Returns the ItemStack for tooltip display.</summary>
        public ItemStack GetItemStack()
        {
            if (m_owner == null) return null;
            var stack = m_owner.PlayerInventory.Inventory.GetSlot(m_slotIndex);
            return (stack != null && stack.Valid) ? stack : null;
        }
    }
}
