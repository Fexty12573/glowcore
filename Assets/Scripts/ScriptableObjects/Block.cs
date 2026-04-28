using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Block", menuName = "Scriptable Objects/Block")]
    public class Block : Item
    {
        public GameObject NodeToBuild;
        public float BuildRadius = 4f;
    }
}