using GlowCore.World;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolBehaviour : MonoBehaviour, IHandItem
{
    private bool m_isHolding;

    public Tool Tool;
    
    
    public void Use(InputValue value)
    {
        Node actionNode = NodeActionSystem.Instance.CurrentNode;
        if (actionNode is null)
            return;
        m_isHolding = value.Get<float>() >= 0.5f;
        if (m_isHolding)
        {
            actionNode.StartHold();
        }
        else
        {
            actionNode.EndHold();
        }
    }

    void Start()
    {
        if (!Tool.Prefab.TryGetComponent(out ToolBehaviour toolBehaviour))
        {
            Debug.LogError($"Tool {Tool.Name} has no ToolBehaviour Component.");
        }
    }
    
    void Update()
    {
        Node actionNode = NodeActionSystem.Instance?.CurrentNode;
        if (actionNode is not null && m_isHolding)
        {
            actionNode.UpdateHold(Time.deltaTime);
        }
    }
}
