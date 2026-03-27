using System.Linq;
using System.ComponentModel;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolBehaviour : MonoBehaviour, IHandItem
{
    bool m_isHolding;
    [SerializeField] Node m_selectedNode;

    [SerializeField][ReadOnly(true)] private Tool Tool;

    public void Use(InputValue value)
    {
        if (m_selectedNode is null)
            return;
        if (m_selectedNode.NodeData.UsableTools.All(t => t.Tool != Tool))
            return;
        m_isHolding = value.Get<float>() >= 0.5f;
        if (m_isHolding)
        {
            m_selectedNode.StartHold();
        }
        else
        {
            m_selectedNode.EndHold();
        }
    }

    private void Start()
    {
        if (!Tool.Prefab.TryGetComponent(out ToolBehaviour toolBehaviour))
        {
            Debug.LogError($"Tool {Tool.Name} has no ToolBehaviour Component.");
        }
    }

    private void Update()
    {
        if (m_isHolding)
        {
            m_selectedNode?.UpdateHold(Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        NodeActionSystem.OnChangeSelectedNode += HandleNodeChanged;
    }

    private void OnDisable()
    {
        NodeActionSystem.OnChangeSelectedNode -= HandleNodeChanged;
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