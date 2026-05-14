using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Inventory
{
    public class RecipeRowUI : MonoBehaviour
    {
        private const float kIngredientRowHeight = 20f;
        private const float kIngredientIconSize = 14f;
        private const float kIngredientSpacing = 2f;
        private const int kIngredientFontSize = 9;

        [Header("References")]
        [SerializeField] private Image m_background;
        [SerializeField] private Image m_resultIcon;
        [SerializeField] private TextMeshProUGUI m_recipeName;
        [SerializeField] private Transform m_ingredientParent;
        [SerializeField] private Button m_craftButton;
        [SerializeField] private Image m_craftButtonImage;

        [Header("Backgrounds")]
        [SerializeField] private Sprite m_defaultBackground;
        [SerializeField] private Sprite m_craftableBackground;

        [Header("Craft Button")]
        [SerializeField] private Sprite m_craftButtonActive;
        [SerializeField] private Sprite m_craftButtonInactive;

        private ICraftingService m_craftingService;
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

        public void Initialize(ICraftingService craftingService, Recipe recipe, TooltipUI tooltip, RecipeRowTheme theme = null)
        {
            m_craftingService = craftingService;
            m_recipe = recipe;

            if (theme != null)
                ApplyTheme(theme);

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
                labelLayout.preferredHeight = kIngredientRowHeight;

                var hlg = labelGo.GetComponent<HorizontalLayoutGroup>();
                hlg.spacing = kIngredientSpacing;
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

                var iconLayout = iconGo.AddComponent<LayoutElement>();
                iconLayout.preferredWidth = kIngredientIconSize;
                iconLayout.preferredHeight = kIngredientIconSize;

                var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                textGo.transform.SetParent(labelGo.transform, false);
                var tmp = textGo.GetComponent<TextMeshProUGUI>();
                tmp.fontSize = kIngredientFontSize;
                tmp.raycastTarget = false;
                tmp.enableAutoSizing = false;

                var textLayout = textGo.AddComponent<LayoutElement>();
                textLayout.preferredHeight = kIngredientIconSize;

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
            if (m_craftingService == null || m_recipe == null)
                return;

            var canCraft = m_craftingService.CanCraft(m_recipe);

            for (var i = 0; i < m_ingredientLabels.Length; i++)
            {
                var label = m_ingredientLabels[i];
                var have = m_craftingService.GetItemCount(label.Ingredient.Item);
                var need = label.Ingredient.Amount;
                var enough = have >= need;

                label.Text.text = $"{label.Ingredient.Item.Name} {have}/{need}";
                label.Text.color = enough ? (Color)UIColors.Green : (Color)UIColors.MissingMat;
            }

            m_craftButton.interactable = canCraft;

            if (m_background != null)
                m_background.sprite = canCraft ? m_craftableBackground : m_defaultBackground;

            if (m_craftButtonImage != null)
                m_craftButtonImage.sprite = canCraft ? m_craftButtonActive : m_craftButtonInactive;
        }

        private void OnCraftClicked()
        {
            m_craftingService.Craft(m_recipe);
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
        }

        // A theme overrides only the sprites it actually sets — a null sprite leaves the
        // prefab default in place, so a station can re-skin just the parts it wants.
        private void ApplyTheme(RecipeRowTheme theme)
        {
            if (theme.DefaultBackground != null)
                m_defaultBackground = theme.DefaultBackground;
            if (theme.CraftableBackground != null)
                m_craftableBackground = theme.CraftableBackground;
            if (theme.CraftButtonActive != null)
                m_craftButtonActive = theme.CraftButtonActive;
            if (theme.CraftButtonInactive != null)
                m_craftButtonInactive = theme.CraftButtonInactive;
        }
    }
}
