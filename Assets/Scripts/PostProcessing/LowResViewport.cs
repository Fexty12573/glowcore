using UnityEngine;
using UnityEngine.UI;

public class LowResViewport : MonoBehaviour
{
    [SerializeField] private Camera m_sourceCamera;
    [SerializeField] private RawImage m_outputImage;
    [SerializeField] private int m_outputHeight;

    private AspectRatioFitter m_aspectRatioFitter;
    private RenderTexture m_renderTexture;
    private int m_lastScreenWidth;
    private int m_lastScreenHeight;

    private void Start()
    {
        m_aspectRatioFitter = GetComponentInChildren<AspectRatioFitter>();
        if (m_aspectRatioFitter == null)
        {
            Debug.LogError("AspectRatioFitter component not found in children.");
            return;
        }

        ApplyAspect();
    }

    private void Update()
    {
        if (m_aspectRatioFitter == null)
            return;

        if (Screen.width == m_lastScreenWidth && Screen.height == m_lastScreenHeight)
            return;

        ApplyAspect();
    }

    private void OnDestroy() => ReleaseRenderTexture();

    private void ApplyAspect()
    {
        m_lastScreenWidth = Screen.width;
        m_lastScreenHeight = Screen.height;

        var aspectRatio = (float)m_lastScreenWidth / m_lastScreenHeight;
        var resolution = GetOutputResolution(aspectRatio);

        ReleaseRenderTexture();

        m_renderTexture = new RenderTexture(resolution.x, resolution.y, 24)
        {
            filterMode = FilterMode.Point,
            useMipMap = false,
            autoGenerateMips = false,
            antiAliasing = 1
        };

        m_sourceCamera.targetTexture = m_renderTexture;
        m_outputImage.texture = m_renderTexture;
        m_outputImage.color = Color.white;
        m_aspectRatioFitter.aspectRatio = aspectRatio;
    }

    private void ReleaseRenderTexture()
    {
        if (m_renderTexture == null)
            return;

        if (m_sourceCamera != null && m_sourceCamera.targetTexture == m_renderTexture)
            m_sourceCamera.targetTexture = null;

        m_renderTexture.Release();
        Destroy(m_renderTexture);
        m_renderTexture = null;
    }

    private Vector2Int GetOutputResolution(float aspectRatio)
    {
        var height = m_outputHeight;
        var width = Mathf.RoundToInt(height * aspectRatio);

        return new Vector2Int(width, height);
    }
}
