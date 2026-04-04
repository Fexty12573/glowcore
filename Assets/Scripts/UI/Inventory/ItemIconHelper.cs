using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public static class ItemIconHelper
    {
        public static void ApplyIconAndCount(Image icon, TMP_Text countText, SlotData data, bool show)
        {
            var sprite = show ? GetSprite(data.Item) : null;

            if (icon != null)
            {
                icon.enabled = sprite != null;
                if (sprite != null)
                    icon.sprite = sprite;
            }

            if (countText != null)
            {
                countText.enabled = show && data.Amount > 1;
                if (show && data.Amount > 1)
                    countText.text = data.Amount.ToString();
            }
        }

        public static Sprite GetSprite(Item item) => item?.Icon;
    }
}
