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

        private IInventoryService m_service;
        private int m_hotbarIndex;
        private bool m_isSelected;
        private bool m_isHovered;
        private bool m_hasItem;

        public void Initialize(IInventoryService service, int hotbarIndex)
        {
            m_service = service;
            m_hotbarIndex = hotbarIndex;

            if (m_keyLabel != null)
            {
                m_keyLabel.text = (hotbarIndex + 1).ToString();
                m_keyLabel.color = UIColors.WhiteFaint;
            }

            Refresh(m_service.GetSlotData(hotbarIndex));
        }

        public void Refresh(SlotData data)
        {
            m_hasItem = data.IsValid;
            ItemIconHelper.ApplyIconAndCount(m_icon, m_countText, data, m_hasItem);
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
                m_service?.SelectHotbarSlot(m_hotbarIndex);
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
