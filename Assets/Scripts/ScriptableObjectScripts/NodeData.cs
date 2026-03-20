using UnityEngine;

namespace ScriptableObjectScripts
{
    [CreateAssetMenu(menuName = "Interactables/InteractableData")]
    public class NodeData : ScriptableObject
    {
        [SerializeField] public float InteractRange;
        [SerializeField] public ItemDrop[] ItemDrops;
        [SerializeField] public float BreakTime;
        [SerializeField] public ToolData[] UsableTools;
    }

    [System.Serializable]
    public class ItemDrop
    {
        public ItemData item;
        public int min;
        public int max;
    }
}