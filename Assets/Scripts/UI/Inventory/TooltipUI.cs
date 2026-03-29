using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlowCore.UI.Inventory
{
    /// <summary>Floating tooltip that follows the mouse cursor and displays item info.</summary>
    public class TooltipUI : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_nameText;
        [SerializeField] private TextMeshProUGUI m_descriptionText;
        [SerializeField] private Canvas m_parentCanvas;

        private const float kMouseOffset = 14f;
        private bool m_isVisible;

        private void Awake()
        {
            if (m_canvasGroup != null)
            {
                m_canvasGroup.alpha = 0f;
                m_canvasGroup.blocksRaycasts = false;
                m_canvasGroup.interactable = false;
            }

            // Pivot top-left so the tooltip expands to the right and downward from the anchor point
            if (m_rectTransform != null)
                m_rectTransform.pivot = new Vector2(0f, 1f);
        }

        public void Show(ItemStack stack)
        {
            if (stack == null || !stack.Valid) return;
            ShowItem(stack.Item);
        }

        public void Show(Item item)
        {
            if (item == null) return;
            ShowItem(item);
        }

        private void ShowItem(Item item)
        {
            if (m_nameText != null)
                m_nameText.text = item.Name;

            if (m_descriptionText != null)
            {
                var desc = item.Description;
                m_descriptionText.text = string.IsNullOrEmpty(desc) ? "" : desc;
                m_descriptionText.enabled = !string.IsNullOrEmpty(desc);
            }

            m_isVisible = true;

            if (m_canvasGroup != null)
                m_canvasGroup.alpha = 1f;

            UpdatePosition();
        }

        public void Hide()
        {
            m_isVisible = false;

            if (m_canvasGroup != null)
                m_canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (m_isVisible)
                UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (m_rectTransform == null || m_parentCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_parentCanvas.transform as RectTransform,
                Mouse.current.position.ReadValue(),
                m_parentCanvas.worldCamera,
                out var localPoint);

            localPoint.x += kMouseOffset;
            localPoint.y -= kMouseOffset;
            m_rectTransform.localPosition = localPoint;
        }
    }
}
