using GlowCore.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPromptSystem : MonoBehaviour
{
    private static ActionPromptSystem s_instance;

    [SerializeField] private Camera m_camera;

    [Header("ActionPrompt")]
    [SerializeField] private Vector3 m_promptOffset;
    [SerializeField] private GameObject m_promptUI; // An Action Prompt is a visual UI Element that pops up when the player hovers over a Node and says e.g. "Break with Leftclick".
    [SerializeField] private Vector3 m_rotatePromptOffset;
    [SerializeField] private GameObject m_rotatePromptUI;
    [SerializeField] private TextMeshProUGUI m_promptText;
    [SerializeField] private GameObject m_leftClickIcon;
    [SerializeField] private GameObject m_eButtonIcon;

    public static ActionPromptSystem Instance => s_instance;

    [Header("ProgressBar")]
    [SerializeField] private Vector3 m_progressBarOffset;
    [SerializeField] private Slider m_nodeBreakProgressBar;
    private Node m_currentNode;
    private bool m_isInteractableNode;
    private bool m_showBreakNodePrompt; // Only shown on the first Node that the player breaks.
    private bool m_isBreaking;
    private Vector3? m_rotatePromptPosition; // Null means that it is turned off

    public void EnableRotatePrompt(Vector3 position)
    {
        m_rotatePromptPosition = position + m_rotatePromptOffset;
        m_rotatePromptUI.SetActive(true);
        UpdateRotatePromptPosition();
    }

    public void DisableRotatePrompt()
    {
        m_rotatePromptPosition = null;
        m_rotatePromptUI.SetActive(false);
    }

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("ActionPromptSystem: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        s_instance = this;
    }

    private void Update()
    {
        if (m_rotatePromptPosition is not null)
            UpdateRotatePromptPosition();

        if (m_currentNode == null || m_currentNode.MarkedForDeletion)
            return;

        if (m_isBreaking)
        {
            UpdateNodeBreakProgress();
            return;
        }
        UpdatePromptPosition();
    }

    private void Start()
    {
        m_promptUI.SetActive(false);
        if (NodeActionSystem.Instance == null)
        {
            Debug.LogError("ActionPromptSystem didn't find the Singleton instance of NodeActionSystem (it was null)");
            return;
        }
        NodeActionSystem.Instance.OnChangeSelectedNode += HandleNodeChanged;
        Node.OnStartBreaking += HandleNodeStartBreaking;
        Node.OnCancelBreaking += HandleNodeCancelBreaking;
        Node.OnNodeBroken += HandleNodeBroken;
    }

    private void OnDestroy()
    {
        // Node events are static — they outlive the scene, so unsubscribe unconditionally.
        // A skipped unsubscribe leaves this destroyed instance handling events in the next
        // scene and touching its destroyed progress bar (MissingReferenceException).
        Node.OnStartBreaking -= HandleNodeStartBreaking;
        Node.OnCancelBreaking -= HandleNodeCancelBreaking;
        Node.OnNodeBroken -= HandleNodeBroken;

        if (NodeActionSystem.Instance != null)
            NodeActionSystem.Instance.OnChangeSelectedNode -= HandleNodeChanged;
    }

    private void HandleNodeChanged(Node node)
    {
        DisableProgressBar();
        if (node == null || node.IsHolding) //Only a machine can already be breaking this Node when this event is fired.
        {
            DisablePrompt();
            return;
        }
        EnablePrompt(node);
    }

    private void EnablePrompt(Node node)
    {
        m_currentNode = node;
        m_promptUI.SetActive(true);
        if (node.TryGetComponent<IInteractable>(out var interactable))
        {
            m_promptText.text = interactable.GetActionPromptText();
            m_leftClickIcon.SetActive(false);
            m_eButtonIcon.SetActive(interactable is not Sign);
        }
        else
        {
            m_promptText.text = "Break";
            m_leftClickIcon.SetActive(true);
            m_eButtonIcon.SetActive(false);
        }
        UpdatePromptPosition();
    }

    private void DisablePrompt()
    {
        m_currentNode = null;
        m_promptUI.SetActive(false);
    }

    private void UpdatePromptPosition()
    {
        Vector3 promptPosition = m_currentNode.transform.position + m_promptOffset;
        m_promptUI.transform.position = m_camera.WorldToScreenPoint(promptPosition);
    }

    private void UpdateNodeBreakProgress()
    {
        Vector3 progressBarPosition = m_currentNode.transform.position + m_progressBarOffset;
        m_nodeBreakProgressBar.transform.position = m_camera.WorldToScreenPoint(progressBarPosition);
        m_nodeBreakProgressBar.value = m_currentNode.GetBreakProgress();
    }

    private void HandleNodeStartBreaking(Node node, bool byPlayer)
    {
        if (node != m_currentNode || !byPlayer)
            return;

        m_isBreaking = true;
        m_nodeBreakProgressBar.gameObject.SetActive(true);
        m_promptUI.SetActive(false);
        UpdateNodeBreakProgress();
    }

    private void HandleNodeCancelBreaking(Node node, bool byPlayer)
    {
        if (node != m_currentNode || !byPlayer)
            return;

        DisableProgressBar();
        if (!node.MarkedForDeletion && m_currentNode == node)
            m_promptUI.SetActive(true);
    }

    private void HandleNodeBroken(Node node, bool byPlayer)
    {
        if (byPlayer)
            DisableProgressBar();
    }

    private void DisableProgressBar()
    {
        m_isBreaking = false;
        m_nodeBreakProgressBar.gameObject.SetActive(false);
    }

    private void UpdateRotatePromptPosition()
    {
        m_rotatePromptUI.transform.position = m_camera.WorldToScreenPoint(m_rotatePromptPosition!.Value);
    }
}
