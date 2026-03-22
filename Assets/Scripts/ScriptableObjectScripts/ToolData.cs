using UnityEngine;

namespace ScriptableObjectScripts
{
    [CreateAssetMenu(menuName = "Items/Tool")]
    public class ToolData : ScriptableObject
    {
        [SerializeField] private float m_breakMultiplier;
    }
}