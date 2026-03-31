using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public static class ItemIconHelper
    {
        private static readonly Dictionary<Texture2D, Sprite> s_cache = new Dictionary<Texture2D, Sprite>();

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

        public static Sprite GetSprite(Item item)
        {
            if (item == null || item.Icon == null)
                return null;

            if (s_cache.TryGetValue(item.Icon, out var cached))
                return cached;

            var sprite = Sprite.Create(
                item.Icon,
                new Rect(0, 0, item.Icon.width, item.Icon.height),
                new Vector2(0.5f, 0.5f));

            s_cache[item.Icon] = sprite;
            return sprite;
        }
    }
}
