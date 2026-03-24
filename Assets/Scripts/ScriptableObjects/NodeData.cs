using System;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Nodes/NodeData")]
    public class NodeData : ScriptableObject
    {
        public float InteractionRange;
        public ItemDrop[] ItemDrops;
        public float BreakTime;
        public bool IsIndestructible;
        public UsableTool[] UsableTools;

        public float GetBreakTime(Tool usedTool)
        {
            if (!usedTool)
                return BreakTime;

            var usableTool = UsableTools.SingleOrDefault(tool => tool.Tool == usedTool);
            if (usableTool is not null)
                return BreakTime * usableTool.BreakMultiplier;

            return BreakTime;
        }
    }

    [Serializable]
    public class ItemDrop
    {
        public Item Item;
        [Min(0)] public int Min;
        [Min(0)] public int Max;
    }

    [Serializable]
    public class UsableTool
    {
        public Tool Tool;
        public float BreakMultiplier;
    }
}