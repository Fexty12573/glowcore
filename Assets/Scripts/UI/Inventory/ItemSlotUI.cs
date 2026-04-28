using GlowCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    [RequireComponent(typeof(Image))]
    public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        private SlotData m_currentData;

        public int SlotIndex => m_slotIndex;

        public void Initialize(InventoryUI owner, int slotIndex, SlotData initialData)
        {
            m_owner = owner;
            m_slotIndex = slotIndex;

            if (m_keyLabel != null)
                m_keyLabel.enabled = false;

            Refresh(initialData);
        }

        public void SetGhosted(bool ghosted)
        {
            m_isGhosted = ghosted;
            ApplyVisuals();
        }

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

        public void Refresh(SlotData data)
        {
            m_currentData = data;
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            var showItem = m_currentData.IsValid && !m_isGhosted;
            ItemIconHelper.ApplyIconAndCount(m_icon, m_countText, m_currentData, showItem);
            UpdateBorderColor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_owner?.OnSlotPressed(m_slotIndex);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_owner?.OnSlotReleased();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Required so OnDrag/OnEndDrag fire — pickup already happened in OnPointerDown
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_owner?.OnDragUpdate();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                m_owner?.OnSlotReleased();
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
            if (m_border == null)
                return;

            if (m_isHovered)
                m_border.color = UIColors.SlotHoverBorder;
            else if (m_isHotbarSlot)
                m_border.color = UIColors.WoodBorderLight;
            else
                m_border.color = UIColors.SlotBorder;

            if (m_background != null)
            {
                var hasItem = m_currentData.IsValid && !m_isGhosted;

                if (m_isHovered)
                    m_background.color = hasItem ? UIColors.SlotFilledBg : UIColors.SlotHoverBg;
                else
                    m_background.color = hasItem ? UIColors.SlotFilledBg : UIColors.SlotBg;
            }
        }

        public SlotData GetSlotData() => m_currentData;
    }
}
