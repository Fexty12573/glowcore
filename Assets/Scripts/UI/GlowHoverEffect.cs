using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class GlowHoverEffect : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler
    {
        private const float kLerpSpeed = 14f;
        private const float kPressScale = 0.97f;

        private static readonly Color kDisabledTint = new Color(0.94f, 0.90f, 0.83f, 0.2f);

        [SerializeField] private float m_hoverScale = 1.05f;
        [SerializeField] private Color m_hoverOutlineColor = new Color(1f, 0.92f, 0.66f, 0.95f);
        [SerializeField] private Vector2 m_hoverOutlineDistance = new Vector2(3f, -3f);

        private RectTransform m_rect;
        private Button m_button;
        private Image m_image;
        private UnityEngine.UI.Outline m_outline;
        private Vector3 m_baseScale;
        private Color m_baseImageColor;
        private Color m_baseOutlineColor;
        private Vector2 m_baseOutlineDistance;
        private bool m_hasOutlineBaseline;
        private bool m_isHovering;
        private bool m_isPressed;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            m_isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData) => m_isHovering = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable())
                return;
            m_isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData) => m_isPressed = false;

        public void OnSelect(BaseEventData eventData)
        {
            if (!IsInteractable())
                return;
            m_isHovering = true;
        }

        public void OnDeselect(BaseEventData eventData) => m_isHovering = false;

        private void Awake()
        {
            m_rect = GetComponent<RectTransform>();
            m_button = GetComponent<Button>();
            m_image = GetComponent<Image>();
            m_outline = GetComponent<UnityEngine.UI.Outline>();
            m_baseScale = m_rect.localScale;
            if (m_image != null)
                m_baseImageColor = m_image.color;
            if (m_outline != null)
            {
                m_baseOutlineColor = m_outline.effectColor;
                m_baseOutlineDistance = m_outline.effectDistance;
                m_hasOutlineBaseline = true;
            }
        }

        private void OnDisable()
        {
            m_isHovering = false;
            m_isPressed = false;
            m_rect.localScale = m_baseScale;
            if (m_image != null)
                m_image.color = m_baseImageColor;
            if (m_hasOutlineBaseline)
            {
                m_outline.effectColor = m_baseOutlineColor;
                m_outline.effectDistance = m_baseOutlineDistance;
            }
        }

        private void Update()
        {
            bool interactable = IsInteractable();
            if (!interactable)
            {
                m_isHovering = false;
                m_isPressed = false;
            }

            float t = 1f - Mathf.Exp(-kLerpSpeed * Time.unscaledDeltaTime);

            float scaleFactor = m_isPressed ? kPressScale : (m_isHovering ? m_hoverScale : 1f);
            m_rect.localScale = Vector3.Lerp(m_rect.localScale, m_baseScale * scaleFactor, t);

            if (m_image != null)
            {
                Color targetImageColor = interactable ? m_baseImageColor : kDisabledTint;
                m_image.color = Color.Lerp(m_image.color, targetImageColor, t);
            }

            if (m_hasOutlineBaseline)
            {
                Color targetColor = m_isHovering ? m_hoverOutlineColor : m_baseOutlineColor;
                Vector2 targetDistance = m_isHovering ? m_hoverOutlineDistance : m_baseOutlineDistance;
                m_outline.effectColor = Color.Lerp(m_outline.effectColor, targetColor, t);
                m_outline.effectDistance = Vector2.Lerp(m_outline.effectDistance, targetDistance, t);
            }
        }

        private bool IsInteractable() => m_button == null || m_button.IsInteractable();
    }
}
