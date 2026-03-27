using GlowCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    /// <summary>Single slot in the always-visible hotbar. Displays item icon, count, key number, and selection highlight.</summary>
    [RequireComponent(typeof(Image))]
    public class HotbarSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image m_background;
        [SerializeField] private Image m_icon;
        [SerializeField] private TextMeshProUGUI m_countText;
        [SerializeField] private TextMeshProUGUI m_keyLabel;
        [SerializeField] private Image m_border;

        private PlayerInventory m_playerInventory;
        private int m_hotbarIndex;
        private bool m_isSelected;
        private bool m_isHovered;

        public void Initialize(PlayerInventory playerInventory, int hotbarIndex)
        {
            m_playerInventory = playerInventory;
            m_hotbarIndex = hotbarIndex;

            if (m_keyLabel != null)
            {
                m_keyLabel.text = (hotbarIndex + 1).ToString();
                m_keyLabel.color = UIColors.WhiteFaint;
            }

            Refresh();
        }

        public void Refresh()
        {
            if (m_playerInventory == null) return;

            var stack = m_playerInventory.GetHotbarSlot(m_hotbarIndex);
            var hasItem = stack != null && stack.Valid;
            var sprite = hasItem ? ItemIconHelper.GetSprite(stack.Item) : null;

            if (m_icon != null)
            {
                m_icon.enabled = sprite != null;
                if (sprite != null)
                    m_icon.sprite = sprite;
            }

            if (m_countText != null)
            {
                m_countText.enabled = hasItem && stack.Amount > 1;
                if (hasItem && stack.Amount > 1)
                    m_countText.text = stack.Amount.ToString();
            }

            if (m_background != null)
                m_background.color = hasItem ? UIColors.SlotFilledBg : UIColors.SlotBg;

            UpdateBorderColor();
        }

        public void SetSelected(bool selected)
        {
            m_isSelected = selected;
            UpdateBorderColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_playerInventory.SelectHotbarSlot(m_hotbarIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isHovered = true;
            UpdateBorderColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isHovered = false;
            UpdateBorderColor();
        }

        private void UpdateBorderColor()
        {
            if (m_border == null) return;

            if (m_isSelected)
                m_border.color = UIColors.SelectedBorder;
            else if (m_isHovered)
                m_border.color = UIColors.SlotHoverBorder;
            else
                m_border.color = UIColors.SlotBorder;
        }
    }
}
