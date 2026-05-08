using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class SceneTransitionOverlay : MonoBehaviour
    {
        private const float kFadeInSeconds = 0.25f;
        private const float kFadeOutSeconds = 0.25f;
        private const float kMinHoldSeconds = 0.55f;
        private const float kPostLoadHoldSeconds = 0.05f;
        private const float kPulseFrequency = 2.0f;
        private const float kPulseMin = 0.55f;
        private const float kPulseMax = 1.0f;
        private const int kEmberCount = 9;

        private static SceneTransitionOverlay s_instance;
        private static TMP_FontAsset s_titleFont;
        private static TMP_FontAsset s_subtitleFont;

        private CanvasGroup m_canvasGroup;
        private TextMeshProUGUI m_titleText;
        private TextMeshProUGUI m_subtitleText;
        private bool m_isAnimating;

        public static SceneTransitionOverlay Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = Build();
                return s_instance;
            }
        }

        public static void ConfigureFonts(TMP_FontAsset titleFont, TMP_FontAsset subtitleFont)
        {
            s_titleFont = titleFont;
            s_subtitleFont = subtitleFont;
            if (s_instance != null)
                s_instance.ApplyFonts();
        }

        public Coroutine LoadScene(string sceneName, string subtitle = "Awakening...")
        {
            return StartCoroutine(Run(sceneName, subtitle));
        }

        private void ApplyFonts()
        {
            if (m_titleText != null && s_titleFont != null)
                m_titleText.font = s_titleFont;
            if (m_subtitleText != null && s_subtitleFont != null)
                m_subtitleText.font = s_subtitleFont;
        }

        private void Update()
        {
            if (m_titleText == null || m_canvasGroup == null || m_canvasGroup.alpha < 0.05f)
                return;

            var pulse = Mathf.Lerp(kPulseMin, kPulseMax, 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * kPulseFrequency));
            Color c = m_titleText.color;
            c.a = pulse * m_canvasGroup.alpha;
            m_titleText.color = c;
        }

        private IEnumerator Run(string sceneName, string subtitle)
        {
            if (m_isAnimating)
                yield break;
            m_isAnimating = true;

            if (m_subtitleText != null)
                m_subtitleText.text = subtitle;

            m_canvasGroup.blocksRaycasts = true;
            yield return Fade(0f, 1f, kFadeInSeconds);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            var startTime = Time.unscaledTime;
            while (op.progress < 0.9f)
                yield return null;

            var elapsed = Time.unscaledTime - startTime;
            if (elapsed < kMinHoldSeconds)
                yield return new WaitForSecondsRealtime(kMinHoldSeconds - elapsed);

            op.allowSceneActivation = true;
            while (!op.isDone)
                yield return null;

            if (kPostLoadHoldSeconds > 0f)
                yield return new WaitForSecondsRealtime(kPostLoadHoldSeconds);

            yield return Fade(1f, 0f, kFadeOutSeconds);
            m_canvasGroup.blocksRaycasts = false;
            m_isAnimating = false;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                m_canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
                yield return null;
            }
            m_canvasGroup.alpha = to;
        }

        private static SceneTransitionOverlay Build()
        {
            var root = new GameObject("[SceneTransitionOverlay]");
            DontDestroyOnLoad(root);

            var canvasGO = new GameObject("Canvas", typeof(RectTransform));
            canvasGO.transform.SetParent(root.transform, false);
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(854f, 480f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            var bg = new GameObject("Background", typeof(RectTransform));
            bg.transform.SetParent(canvasGO.transform, false);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.sprite = BuildGradientSprite();
            bgImg.type = Image.Type.Simple;
            bgImg.color = Color.white;
            bgImg.raycastTarget = true;
            FullStretch((RectTransform)bg.transform);

            BuildEmbers(canvasGO.transform);

            var titleGO = new GameObject("Title", typeof(RectTransform));
            titleGO.transform.SetParent(canvasGO.transform, false);
            var title = titleGO.AddComponent<TextMeshProUGUI>();
            title.text = "GLOWCORE";
            title.fontSize = 64f;
            title.alignment = TextAlignmentOptions.Center;
            title.color = (Color)UIColors.Accent;
            title.fontStyle = FontStyles.Bold;
            title.raycastTarget = false;
            if (s_titleFont != null)
                title.font = s_titleFont;

            var titleRect = (RectTransform)titleGO.transform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 20f);
            titleRect.sizeDelta = new Vector2(720f, 90f);

            var subGO = new GameObject("Subtitle", typeof(RectTransform));
            subGO.transform.SetParent(canvasGO.transform, false);
            var sub = subGO.AddComponent<TextMeshProUGUI>();
            sub.text = "Awakening...";
            sub.fontSize = 14f;
            sub.alignment = TextAlignmentOptions.Center;
            sub.color = (Color)UIColors.AccentDim;
            sub.characterSpacing = 8f;
            sub.fontStyle = FontStyles.UpperCase;
            sub.raycastTarget = false;
            if (s_subtitleFont != null)
                sub.font = s_subtitleFont;

            var subRect = (RectTransform)subGO.transform;
            subRect.anchorMin = new Vector2(0.5f, 0.5f);
            subRect.anchorMax = new Vector2(0.5f, 0.5f);
            subRect.pivot = new Vector2(0.5f, 0.5f);
            subRect.anchoredPosition = new Vector2(0f, -45f);
            subRect.sizeDelta = new Vector2(400f, 30f);

            var divider = new GameObject("Divider", typeof(RectTransform));
            divider.transform.SetParent(canvasGO.transform, false);
            Image dividerImg = divider.AddComponent<Image>();
            dividerImg.color = (Color)UIColors.AccentDim;
            dividerImg.raycastTarget = false;
            var divRect = (RectTransform)divider.transform;
            divRect.anchorMin = new Vector2(0.5f, 0.5f);
            divRect.anchorMax = new Vector2(0.5f, 0.5f);
            divRect.pivot = new Vector2(0.5f, 0.5f);
            divRect.anchoredPosition = new Vector2(0f, -22f);
            divRect.sizeDelta = new Vector2(140f, 1f);

            var overlay = root.AddComponent<SceneTransitionOverlay>();
            overlay.m_canvasGroup = cg;
            overlay.m_titleText = title;
            overlay.m_subtitleText = sub;
            return overlay;
        }

        private static void FullStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static Sprite BuildGradientSprite()
        {
            const int kHeight = 256;
            var tex = new Texture2D(2, kHeight, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };
            Color top = new Color(0.10f, 0.12f, 0.18f, 1f);
            Color bottom = new Color(0f, 0f, 0f, 1f);
            for (var y = 0; y < kHeight; y++)
            {
                var t = (float)y / (kHeight - 1);
                Color c = Color.Lerp(bottom, top, Mathf.Pow(t, 1.6f));
                tex.SetPixel(0, y, c);
                tex.SetPixel(1, y, c);
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0f, 0f, 2f, kHeight), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite BuildDotSprite()
        {
            const int kSize = 32;
            var tex = new Texture2D(kSize, kSize, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };
            var center = (kSize - 1) * 0.5f;
            var maxDist = center;
            for (var y = 0; y < kSize; y++)
            {
                for (var x = 0; x < kSize; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var d = Mathf.Sqrt(dx * dx + dy * dy) / maxDist;
                    var a = Mathf.Clamp01(1f - d);
                    a = a * a * a;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0f, 0f, kSize, kSize), new Vector2(0.5f, 0.5f), 100f);
        }

        private static void BuildEmbers(Transform parent)
        {
            Sprite dot = BuildDotSprite();
            // Deterministic pseudo-random placement so layout is the same every run.
            int[] xs = { -340, -260, -180, -90, 30, 110, 200, 280, 350 };
            int[] ys = { 80, 130, 180, 90, 200, 60, 150, 110, 170 };
            float[] sizes = { 4f, 6f, 5f, 7f, 4f, 8f, 5f, 6f, 4f };
            float[] alphas = { 0.45f, 0.65f, 0.55f, 0.75f, 0.50f, 0.85f, 0.60f, 0.70f, 0.50f };

            for (var i = 0; i < kEmberCount; i++)
            {
                var emberGO = new GameObject($"Ember{i}", typeof(RectTransform));
                emberGO.transform.SetParent(parent, false);
                Image img = emberGO.AddComponent<Image>();
                img.sprite = dot;
                Color c = (Color)UIColors.Accent;
                c.a = alphas[i];
                img.color = c;
                img.raycastTarget = false;

                var rect = (RectTransform)emberGO.transform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(xs[i], ys[i]);
                rect.sizeDelta = new Vector2(sizes[i], sizes[i]);
            }
        }
    }
}
