using UnityEngine;

public class MapUI : MonoBehaviour
{
    private static MapUI s_instance;

    [SerializeField] private RectTransform m_mapImage;
    [SerializeField] private RectTransform m_playerMarker;
    [SerializeField] private Transform m_playerPosition;

    private readonly float m_worldWidth = 250; // is square
    private bool m_isVisible;

    public static MapUI Instance => s_instance;

    public void SetVisible(bool visible)
    {
        m_isVisible = visible;
        m_mapImage?.gameObject.SetActive(visible);
        Update();
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("MapUI: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        s_instance = this;
    }

    private void Update()
    {
        if (!m_isVisible || m_playerMarker is null || m_playerPosition is null)
            return;

        UpdatePlayerMarker();
    }

    private void UpdatePlayerMarker()
    {
        float mapUIWidth = m_mapImage.rect.size.x; // is square

        float normalizedX = m_playerPosition.position.x / m_worldWidth;
        float normalizedY = m_playerPosition.position.z / m_worldWidth;
        var anchorPosition = new Vector2(normalizedX * mapUIWidth, normalizedY * mapUIWidth); // 0 0 is middle
        m_playerMarker.anchoredPosition = anchorPosition;

        m_playerMarker.localRotation = Quaternion.Euler(0f, 0f, -m_playerPosition.eulerAngles.y);
    }
}
