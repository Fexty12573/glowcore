using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Items/Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] public string ItemName;
        [SerializeField] public GameObject Prefab;
    }
}