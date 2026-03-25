using GlowCore.World;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
}

public class NodeActionSystem : MonoBehaviour
{
    private static NodeActionSystem s_instance;
    
    [SerializeField] private Camera m_camera;
    [SerializeField] private Transform m_player;
    [SerializeField] private float m_raycastRange = 100f;
    private Node m_currentNode;
    private Outline m_currentOutline;
    private Vector2 m_mousePos;
    private bool m_mouseMoved;
    
    public static NodeActionSystem Instance => s_instance;
    public Node CurrentNode => m_currentNode;

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogWarning("NodeActionSystem: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }
    private void Update()
    {
        UpdateOutlineHover();
    }

    private void UpdateOutlineHover()
    {
        if (!m_mouseMoved)
            return;
        m_mouseMoved = false;
        Ray ray = m_camera.ScreenPointToRay(m_mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, m_raycastRange))
        {
            if (!hit.collider.TryGetComponent(out NodeActionChild child))
            {
                Clear();
                return;
            }

            Vector3 playerPos = m_player.position;
            Vector3 hitPos = hit.collider.transform.position;

            playerPos.y = 0;
            hitPos.y = 0;

            float distance = Vector3.Distance(playerPos, hitPos);

            Node node = child.Root;
            Outline outline = node.Outline;

            if (distance > node.GetInteractionRange())
            {
                Clear();
                return;
            }

            if (node != m_currentNode || outline != m_currentOutline)
            {
                Clear();

                m_currentNode = node;
                m_currentOutline = outline;

                if (m_currentOutline)
                    m_currentOutline.enabled = true;
            }
        }
        else
        {
            Clear();
        }
    }

    private void OnInteract(InputValue value)
    {
        m_currentNode?.Interact();
    }

    private void Clear()
    {
        if (m_currentOutline)
            m_currentOutline.enabled = false;

        m_currentNode = null;
        m_currentOutline = null;
    }

    private void OnPoint(InputValue value)
    {
        Vector2 newMousePos = value.Get<Vector2>();
        if (newMousePos != m_mousePos)
        {
            m_mouseMoved = true;
            m_mousePos = newMousePos;
        }
    }
}