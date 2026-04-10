using System.ComponentModel;
using System.Linq;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolBehaviour : MonoBehaviour, IHandItem
{
    bool m_isHolding;
    [SerializeField][ReadOnly(true)] private Node m_selectedNode;
    [SerializeField] private Tool m_tool;

    public void Use(InputValue value)
    {
        // Use == null, not 'is null': destroyed Unity objects are fake-null and bypass 'is null'
        if (m_selectedNode == null)
            return;

        if (m_selectedNode.NodeData.UsableTools.All(t => t.Tool != m_tool))
            return;

        m_isHolding = value.Get<float>() >= 0.5f;
        if (m_isHolding)
            m_selectedNode.StartHold(m_tool);
        else
            m_selectedNode.EndHold();
    }

    private void Start()
    {
        if (!m_tool.Prefab.TryGetComponent(out ToolBehaviour toolBehaviour))
            Debug.LogError($"Tool {m_tool.Name} has no ToolBehaviour Component.");
    }

    private void Update()
    {
        if (m_isHolding && m_selectedNode != null)
            m_selectedNode.UpdateHold(Time.deltaTime);
    }

    private void OnEnable()
    {
        if (NodeActionSystem.Instance == null)
            return;
        NodeActionSystem.Instance.OnChangeSelectedNode += HandleNodeChanged;
        HandleNodeChanged(NodeActionSystem.Instance.CurrentNode);
    }

    private void OnDisable()
    {
        if (NodeActionSystem.Instance == null)
            return;
        NodeActionSystem.Instance.OnChangeSelectedNode -= HandleNodeChanged;
        m_selectedNode?.EndHold();
    }

    private void HandleNodeChanged(Node newNode)
    {
        if (m_selectedNode != newNode)
        {
            m_selectedNode?.EndHold();
            m_isHolding = false;
        }
        m_selectedNode = newNode;
    }
}