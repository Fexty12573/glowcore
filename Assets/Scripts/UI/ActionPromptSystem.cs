using GlowCore.World;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPromptSystem : MonoBehaviour
{
    [Header("ActionPrompt")]
    [SerializeField] private Vector3 m_promptOffset;
    [SerializeField] private GameObject m_promptUI; // An Action Prompt is a visual UI Element that pops up when the player hovers over a Node and says e.g. "Break with Leftclick".
    [SerializeField] private TextMeshProUGUI m_promptText;
    [SerializeField] private GameObject m_leftClickIcon;
    [SerializeField] private GameObject m_eButtonIcon;
    
    [Header("ProgressBar")]
    [SerializeField] private Vector3 m_progressBarOffset;
    [SerializeField] private Slider m_nodeBreakProgressBar;
    private Node m_currentNode;
    private bool m_isInteractableNode;
    private bool m_showBreakNodePrompt; // Only shown on the first Node that the player breaks.
    private bool m_isBreaking;

    private void Update()
    {
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
        m_promptUI.transform.position = Camera.main.WorldToScreenPoint(promptPosition);
    }

    private void UpdateNodeBreakProgress()
    {
        Vector3 progressBarPosition = m_currentNode.transform.position + m_progressBarOffset;
        m_nodeBreakProgressBar.transform.position = Camera.main.WorldToScreenPoint(progressBarPosition);
        m_nodeBreakProgressBar.value = m_currentNode.GetBreakProgress();
    }

    private void HandleNodeStartBreaking(Node node)
    {
        m_isBreaking = true;
        m_nodeBreakProgressBar.gameObject.SetActive(true);
        m_promptUI.SetActive(false);
        UpdateNodeBreakProgress();
    }

    private void HandleNodeCancelBreaking(Node node)
    {
        m_isBreaking = false;
        m_nodeBreakProgressBar.gameObject.SetActive(false);
        if (!node.MarkedForDeletion && m_currentNode == node) 
            m_promptUI.SetActive(true);
    }

    private void HandleNodeBroken(Node node)
    {
        m_isBreaking = false;
        m_nodeBreakProgressBar.gameObject.SetActive(false);
    }
}
