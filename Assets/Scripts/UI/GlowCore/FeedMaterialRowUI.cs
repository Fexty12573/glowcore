using System;
using GlowCore.UI.Inventory;
using GlowCore.World;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Upgrade
{
    public class FeedMaterialRowUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image m_bgImage;
        [SerializeField] private Image m_borderImage;
        [SerializeField] private Image m_icon;
        [SerializeField] private TextMeshProUGUI m_nameLabel;
        [SerializeField] private TextMeshProUGUI m_countLabel;
        [SerializeField] private AmountStepperUI m_stepper;
        [SerializeField] private Button m_addAllButton;

        private IInventoryService m_inventoryService;
        private IGlowCoreObject m_target;
        private Item m_item;
        private int m_required;

        public Item Material => m_item;
        public int SelectedAmount => m_stepper != null ? m_stepper.Value : 0;

        public event Action OnSelectionChanged;

        public void Initialize(IInventoryService inventoryService, IGlowCoreObject target, Item item, int required)
        {
            m_inventoryService = inventoryService;
            m_target = target;
            m_item = item;
            m_required = required;

            if (m_nameLabel != null)
                m_nameLabel.text = item.Name;

            if (m_icon != null)
            {
                var sprite = ItemIconHelper.GetSprite(item);
                if (sprite != null)
                {
                    m_icon.sprite = sprite;
                    m_icon.enabled = true;
                }
                else
                {
                    m_icon.enabled = false;
                }
            }

            if (m_stepper != null)
            {
                m_stepper.OnValueChanged += OnStepperChanged;
                m_stepper.Bind(0, GetMaxSelectable(), 0);
            }

            if (m_addAllButton != null)
                m_addAllButton.onClick.AddListener(OnAddAllClicked);

            Refresh();
        }

        public void ResetSelection()
        {
            if (m_stepper != null)
                m_stepper.SetValue(0);
        }

        public void Refresh()
        {
            if (m_item == null)
                return;

            var accumulated = m_target != null ? m_target.AccumulatedFor(m_item) : 0;
            var have = m_inventoryService != null ? m_inventoryService.CountItem(m_item) : 0;

            if (m_stepper != null)
                m_stepper.SetBounds(0, GetMaxSelectable());

            var pending = m_stepper != null ? m_stepper.Value : 0;

            if (m_countLabel != null)
            {
                m_countLabel.text = $"{accumulated + pending}/{m_required}";
                var enough = accumulated + pending >= m_required;
                m_countLabel.color = enough ? (Color)UIColors.Green : (Color)UIColors.MissingMat;
            }

            if (m_borderImage != null)
            {
                var fullyMet = accumulated >= m_required;
                m_borderImage.color = fullyMet ? (Color)UIColors.Green : (Color)UIColors.SlotBorder;
            }

            if (m_bgImage != null)
            {
                var bg = (Color)UIColors.SlotBg;
                m_bgImage.color = bg;
            }

            if (m_addAllButton != null)
                m_addAllButton.interactable = GetMaxSelectable() > 0;
        }

        private int GetMaxSelectable()
        {
            if (m_target == null || m_inventoryService == null || m_item == null)
                return 0;

            var accumulated = m_target.AccumulatedFor(m_item);
            var stillNeeded = Mathf.Max(0, m_required - accumulated);
            var have = m_inventoryService.CountItem(m_item);
            return Mathf.Min(stillNeeded, have);
        }

        private void OnDestroy()
        {
            if (m_stepper != null)
                m_stepper.OnValueChanged -= OnStepperChanged;

            if (m_addAllButton != null)
                m_addAllButton.onClick.RemoveListener(OnAddAllClicked);
        }

        private void OnAddAllClicked()
        {
            if (m_stepper != null)
                m_stepper.SetValue(GetMaxSelectable());
        }

        private void OnStepperChanged(int value)
        {
            Refresh();
            OnSelectionChanged?.Invoke();
        }
    }
}
