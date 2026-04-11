using UnityEngine;
using UnityEngine.UI;

public class LowResViewport : MonoBehaviour
{
    [SerializeField] private Camera m_sourceCamera;
    [SerializeField] private RawImage m_outputImage;
    [SerializeField] private int m_outputWidth;
    [SerializeField] private int m_outputHeight;

    private RenderTexture m_renderTexture;

    private void Start()
    {
        m_renderTexture = new RenderTexture(m_outputWidth, m_outputHeight, 24)
        {
            filterMode = FilterMode.Point,
            useMipMap = false,
            autoGenerateMips = false,
            antiAliasing = 1
        };

        m_sourceCamera.targetTexture = m_renderTexture;
        m_outputImage.texture = m_renderTexture;
        m_outputImage.color = Color.white;
    }

    private void OnDestroy()
    {
        if (m_renderTexture == null)
            return;

        if (m_sourceCamera != null && m_sourceCamera.targetTexture == m_renderTexture)
            m_sourceCamera.targetTexture = null;

        m_renderTexture.Release();
        Destroy(m_renderTexture);
    }
}
