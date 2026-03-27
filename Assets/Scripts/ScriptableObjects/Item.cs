using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject
    {
        public string Name;
        [TextArea(1, 3)] public string Description;
        public GameObject Prefab;
        public Texture2D Icon;
        public float DropScale = 1f;
        public float InHandScale = 1f;
        [Min(0)] public int MaxStack = 99;
    }
}

