using System.Collections;
using GlowCore.UI.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class EndingScreen : MonoBehaviour, IMenuScreen
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private CanvasGroup m_promptGroup;
        [SerializeField] private Button m_continueButton;
        [SerializeField] private Button m_exitButton;

        [Header("HUD")]
        [SerializeField] private HotbarUI m_hotbar;

        [Header("Timing")]
        [SerializeField][Min(0f)] private float m_fadeInDelaySeconds = 1f;
        [SerializeField][Min(0f)] private float m_fadeInSeconds = 1f;
        [SerializeField][Min(0f)] private float m_promptDelaySeconds = 3f;

        [Header("Camera Zoom")]
        [Tooltip("How far to zoom out: the camera's size/FOV is multiplied by this. 1 = no zoom.")]
        [SerializeField][Min(1f)] private float m_zoomMultiplier = 1.3f;
        [SerializeField][Min(0f)] private float m_zoomSeconds = 2f;

        private IMenuManager m_menuManager;
        private IGameStateController m_gameState;
        private Camera m_camera;
        private Coroutine m_sequence;
        private Coroutine m_zoomRoutine;
        private float m_originalOrthoSize;
        private float m_originalFov;

        public MenuScreenId Id => MenuScreenId.Ending;
        public bool BlocksGameplay => true;

        public void Initialize(IMenuManager menuManager, IGameStateController gameState, Camera gameCamera)
        {
            m_menuManager = menuManager;
            m_gameState = gameState;
            m_camera = gameCamera;

            if (m_continueButton != null)
                m_continueButton.onClick.AddListener(OnContinue);
            if (m_exitButton != null)
                m_exitButton.onClick.AddListener(OnExit);

            m_canvasGroup?.SetVisible(false);
            m_promptGroup?.SetVisible(false);
        }

        public void Show()
        {
            m_hotbar?.SetVisible(false);
            StartZoom();

            if (m_sequence != null)
                StopCoroutine(m_sequence);
            m_sequence = StartCoroutine(PlaySequence());
        }

        public void Hide()
        {
            if (m_sequence != null)
            {
                StopCoroutine(m_sequence);
                m_sequence = null;
            }

            // Clear selection so the last-clicked button is not re-tinted as selected next time.
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            m_canvasGroup?.SetVisible(false);
            m_promptGroup?.SetVisible(false);
            m_hotbar?.SetVisible(true);

            RestoreZoom();
        }

        private void StartZoom()
        {
            if (m_camera == null)
                return;

            m_originalOrthoSize = m_camera.orthographicSize;
            m_originalFov = m_camera.fieldOfView;

            if (m_zoomRoutine != null)
                StopCoroutine(m_zoomRoutine);
            m_zoomRoutine = StartCoroutine(ZoomOut());
        }

        private void RestoreZoom()
        {
            if (m_zoomRoutine != null)
            {
                StopCoroutine(m_zoomRoutine);
                m_zoomRoutine = null;
            }

            if (m_camera == null)
                return;

            m_camera.orthographicSize = m_originalOrthoSize;
            m_camera.fieldOfView = m_originalFov;
        }

        private IEnumerator ZoomOut()
        {
            var ortho = m_camera.orthographic;
            var start = ortho ? m_originalOrthoSize : m_originalFov;
            var end = start * m_zoomMultiplier;

            var elapsed = 0f;
            while (elapsed < m_zoomSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = m_zoomSeconds <= 0f ? 1f : Mathf.SmoothStep(0f, 1f, elapsed / m_zoomSeconds);
                ApplyZoom(Mathf.Lerp(start, end, t), ortho);
                yield return null;
            }

            ApplyZoom(end, ortho);
            m_zoomRoutine = null;
        }

        private void ApplyZoom(float value, bool ortho)
        {
            if (ortho)
                m_camera.orthographicSize = value;
            else
                m_camera.fieldOfView = value;
        }

        private void OnDestroy()
        {
            if (m_continueButton != null)
                m_continueButton.onClick.RemoveListener(OnContinue);
            if (m_exitButton != null)
                m_exitButton.onClick.RemoveListener(OnExit);
        }

        private IEnumerator PlaySequence()
        {
            m_promptGroup?.SetVisible(false);

            var elapsed = 0f;
            while (elapsed < m_fadeInDelaySeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            yield return FadeIn(m_canvasGroup, m_fadeInSeconds);

            elapsed = 0f;
            while (elapsed < m_promptDelaySeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            yield return FadeIn(m_promptGroup, m_fadeInSeconds);

            m_sequence = null;
        }

        private static IEnumerator FadeIn(CanvasGroup group, float seconds)
        {
            if (group == null)
                yield break;

            group.alpha = 0f;
            group.interactable = true;
            group.blocksRaycasts = true;
            if (seconds <= 0f)
            {
                group.alpha = 1f;
                yield break;
            }

            var elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(elapsed / seconds);
                yield return null;
            }

            group.alpha = 1f;
        }

        private void OnContinue()
        {
            m_gameState?.Resume();
            m_menuManager?.CloseAll();
        }

        private void OnExit() => m_gameState?.ReturnToMainMenu();
    }
}
