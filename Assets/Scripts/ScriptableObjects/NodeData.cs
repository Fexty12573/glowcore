using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Nodes/NodeData")]
    public class NodeData : ScriptableObject
    {
        [SerializeField] public float InteractRange;
        [SerializeField] public ItemDrop[] ItemDrops;
        [SerializeField] public float BreakTime;
        [SerializeField] public bool IsIndestructible;
        [SerializeField] public ToolData[] UsableTools;
    }

    [System.Serializable]
    public class ItemDrop
    {
        public ItemData Item;
        public int Min;
        public int Max;
    }
}