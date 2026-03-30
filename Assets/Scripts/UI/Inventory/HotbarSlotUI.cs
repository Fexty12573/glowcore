using GlowCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
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
        private bool m_hasItem;

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
            if (m_playerInventory == null)
                return;

            var stack = m_playerInventory.GetHotbarSlot(m_hotbarIndex);
            m_hasItem = stack != null && stack.Valid;
            var sprite = m_hasItem ? ItemIconHelper.GetSprite(stack.Item) : null;

            if (m_icon != null)
            {
                m_icon.enabled = sprite != null;
                if (sprite != null)
                    m_icon.sprite = sprite;
            }

            if (m_countText != null)
            {
                m_countText.enabled = m_hasItem && stack.Amount > 1;
                if (m_hasItem && stack.Amount > 1)
                    m_countText.text = stack.Amount.ToString();
            }

            UpdateVisuals();
        }

        public void SetSelected(bool selected)
        {
            m_isSelected = selected;
            UpdateVisuals();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_playerInventory.SelectHotbarSlot(m_hotbarIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isHovered = true;
            UpdateVisuals();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isHovered = false;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (m_border != null)
                m_border.enabled = m_isSelected;

            if (m_background != null)
            {
                if (m_isHovered)
                    m_background.color = UIColors.SlotHoverBg;
                else
                    m_background.color = m_hasItem ? UIColors.SlotFilledBg : UIColors.SlotBg;
            }
        }
    }
}
