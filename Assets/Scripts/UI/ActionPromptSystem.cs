using GlowCore.World;
using TMPro;
using UnityEngine;

public class ActionPromptSystem : MonoBehaviour
{
    [SerializeField] private Vector3 m_promptOffset;
    [SerializeField] private GameObject m_promptUI; // An Action Prompt is a visual UI Element that pops up when the player hovers over a Node and says e.g. "Break with Leftclick".
    [SerializeField] private TextMeshProUGUI m_promptText;
    [SerializeField] private GameObject m_leftClickIcon;
    [SerializeField] private GameObject m_eButtonIcon;
    [SerializeField] private TextMeshProUGUI m_nodeBreakProgressBar;
    private Node m_currentNode;
    private bool m_isInteractableNode;
    private bool m_showBreakNodePrompt; // Only shown on the first Node that the player breaks.
    private void Update()
    {
        if (m_currentNode is not null)
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
        if (NodeActionSystem.Instance == null) 
            return;
        NodeActionSystem.Instance.OnChangeSelectedNode -= HandleNodeChanged;
        Node.OnStartBreaking -= HandleNodeStartBreaking;
        Node.OnCancelBreaking -= HandleNodeCancelBreaking;
        Node.OnNodeBroken -= HandleNodeBroken;
    }

    private void HandleNodeChanged(Node node)
    {
        if (node is null)
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
            m_eButtonIcon.SetActive(true);
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
        m_promptUI.transform.position = Camera.main.WorldToScreenPoint(promptPosition);
    }

    private void HandleNodeStartBreaking(Node node)
    {
        Debug.Log("starts");
        m_nodeBreakProgressBar.text = "starts breaking";
    }

    private void HandleNodeCancelBreaking(Node node)
    {
        Debug.Log("stops");
        m_nodeBreakProgressBar.text = "stops breaking";
    }

    private void HandleNodeBroken(Node node)
    {
        Debug.Log("broken");
        m_nodeBreakProgressBar.text = "broken (off)";
    }
}
