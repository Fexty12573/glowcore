using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    /// <summary>Displays a single crafting recipe row: result icon, name, ingredient list, and CRAFT button.</summary>
    public class RecipeRowUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image m_bgImage;
        [SerializeField] private Image m_borderImage;
        [SerializeField] private Image m_resultIcon;
        [SerializeField] private TextMeshProUGUI m_recipeName;
        [SerializeField] private Transform m_ingredientParent;
        [SerializeField] private Button m_craftButton;

        private CraftingUI m_craftingUI;
        private Recipe m_recipe;
        private IngredientLabel[] m_ingredientLabels;

        private sealed class ResultIconTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private TooltipUI m_tooltip;
            private Item m_item;

            public void Initialize(TooltipUI tooltip, Item item)
            {
                m_tooltip = tooltip;
                m_item = item;
            }

            public void OnPointerEnter(PointerEventData eventData) => m_tooltip?.Show(m_item);
            public void OnPointerExit(PointerEventData eventData) => m_tooltip?.Hide();
        }

        private struct IngredientLabel
        {
            public TextMeshProUGUI Text;
            public Image Icon;
            public Recipe.Ingredient Ingredient;
        }

        public void Initialize(CraftingUI craftingUI, Recipe recipe, TooltipUI tooltip)
        {
            m_craftingUI = craftingUI;
            m_recipe = recipe;

            m_recipeName.text = recipe.Name;

            if (m_resultIcon != null)
            {
                var sprite = ItemIconHelper.GetSprite(recipe.ResultItem);
                if (sprite != null)
                {
                    m_resultIcon.sprite = sprite;
                    m_resultIcon.enabled = true;
                }
                else
                {
                    m_resultIcon.enabled = false;
                }

                if (tooltip != null)
                {
                    m_resultIcon.raycastTarget = true;
                    var trigger = m_resultIcon.gameObject.AddComponent<ResultIconTooltipTrigger>();
                    trigger.Initialize(tooltip, recipe.ResultItem);
                }
            }

            BuildIngredientLabels();

            m_craftButton.onClick.AddListener(OnCraftClicked);

            Refresh();
        }

        private void BuildIngredientLabels()
        {
            m_ingredientLabels = new IngredientLabel[m_recipe.Ingredients.Length];

            for (var i = 0; i < m_recipe.Ingredients.Length; i++)
            {
                var ingredient = m_recipe.Ingredients[i];

                var labelGo = new GameObject($"Ingredient_{ingredient.Item.Name}",
                    typeof(RectTransform), typeof(HorizontalLayoutGroup));
                labelGo.transform.SetParent(m_ingredientParent, false);

                var labelLayout = labelGo.AddComponent<LayoutElement>();
                labelLayout.preferredHeight = 20;

                var hlg = labelGo.GetComponent<HorizontalLayoutGroup>();
                hlg.spacing = 2;
                hlg.childAlignment = TextAnchor.MiddleLeft;
                hlg.childForceExpandWidth = false;
                hlg.childForceExpandHeight = false;

                var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconGo.transform.SetParent(labelGo.transform, false);
                var iconImage = iconGo.GetComponent<Image>();
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;
                var iconSprite = ItemIconHelper.GetSprite(ingredient.Item);
                if (iconSprite != null)
                    iconImage.sprite = iconSprite;

                var iconRect = iconGo.GetComponent<RectTransform>();
                var iconLayout = iconGo.AddComponent<LayoutElement>();
                iconLayout.preferredWidth = 14;
                iconLayout.preferredHeight = 14;

                var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                textGo.transform.SetParent(labelGo.transform, false);
                var tmp = textGo.GetComponent<TextMeshProUGUI>();
                tmp.fontSize = 9;
                tmp.raycastTarget = false;
                tmp.enableAutoSizing = false;

                var textLayout = textGo.AddComponent<LayoutElement>();
                textLayout.preferredHeight = 14;

                m_ingredientLabels[i] = new IngredientLabel
                {
                    Text = tmp,
                    Icon = iconImage,
                    Ingredient = ingredient
                };
            }
        }

        public void Refresh()
        {
            if (m_craftingUI == null || m_recipe == null)
                return;

            var canCraft = m_craftingUI.CanCraft(m_recipe);

            for (var i = 0; i < m_ingredientLabels.Length; i++)
            {
                var label = m_ingredientLabels[i];
                var have = m_craftingUI.GetItemCount(label.Ingredient.Item);
                var need = label.Ingredient.Amount;
                var enough = have >= need;

                label.Text.text = $"{label.Ingredient.Item.Name} {have}/{need}";
                label.Text.color = enough ? (Color)UIColors.Green : (Color)UIColors.MissingMat;
            }

            m_craftButton.interactable = canCraft;

            if (m_borderImage != null)
                m_borderImage.color = canCraft ? (Color)UIColors.Green : (Color)UIColors.SlotBorder;

            if (m_bgImage != null)
            {
                var bg = (Color)UIColors.SlotBg;
                bg.a = canCraft ? bg.a : 0.35f;
                m_bgImage.color = bg;
            }
        }

        private void OnCraftClicked()
        {
            m_craftingUI.Craft(m_recipe);
        }
    }
}
