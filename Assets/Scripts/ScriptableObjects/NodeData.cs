using System;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/NodeData")]
    public class NodeData : ScriptableObject
    {
        public float InteractionRange = 3;
        public ItemDrop[] ItemDrops;
        public float BaseBreakTime = 1;
        public bool IsIndestructible;
        public UsableTool[] UsableTools;

        public float GetEffectiveBreakTime(Tool usedTool)
        {
            if (!usedTool)
                return BaseBreakTime;

            var usableTool = UsableTools.SingleOrDefault(tool => tool.Tool == usedTool);
            if (usableTool is not null)
                return BaseBreakTime * usableTool.BreakMultiplier;

            return BaseBreakTime;
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
        public float BreakMultiplier; // 0 means that it can be broken instantly
    }
}