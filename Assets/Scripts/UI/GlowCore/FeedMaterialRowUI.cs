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
        [SerializeField] private Image m_icon;
        [SerializeField] private TextMeshProUGUI m_nameLabel;
        [SerializeField] private TextMeshProUGUI m_countLabel;
        [SerializeField] private Image m_background;

        [Header("Backgrounds")]
        [SerializeField] private Sprite m_defaultBackground;
        [SerializeField] private Sprite m_completeBackground;

        private IInventoryService m_inventoryService;
        private IGlowCoreObject m_target;
        private Item m_item;
        private int m_required;

        public Item Material => m_item;
        public int MaxFeedable => GetMaxSelectable();

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

            Refresh();
        }

        public void Refresh()
        {
            if (m_item == null)
                return;

            var accumulated = m_target != null ? m_target.AccumulatedFor(m_item) : 0;
            var enough = accumulated >= m_required;

            if (m_countLabel != null)
            {
                m_countLabel.text = $"{accumulated}/{m_required}";
                m_countLabel.color = enough ? (Color)UIColors.Green : (Color)UIColors.MissingMat;
            }

            if (m_background != null)
                m_background.sprite = enough ? m_completeBackground : m_defaultBackground;
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
    }
}
