using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Items/Tool")]
    public class ToolData : ScriptableObject
    {
        [SerializeField] private float m_breakMultiplier;
    }
}