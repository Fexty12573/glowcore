using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/Item Registry")]
    public class ItemRegistry : ScriptableObject
    {
        [SerializeField] private List<Item> m_items = new();
        private Dictionary<Guid, Item> m_itemDict = new();

        public IReadOnlyList<Item> Items => m_items;

        private static ItemRegistry s_instance;

        public static ItemRegistry Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Resources.Load<ItemRegistry>("ItemRegistry");
                    if (s_instance == null)
                        Debug.LogError("ItemRegistry asset not found in Resources folder!");

                    s_instance.RebuildLookup();
                }

                return s_instance;
            }
        }

        private void RebuildLookup()
        {
            m_itemDict.Clear();

            foreach (var item in m_items)
                m_itemDict[item.Id] = item;
        }

#if UNITY_EDITOR
        public void RebuildRegistry()
        {
            m_items.Clear();

            var guids = AssetDatabase.FindAssets("t:Item", new[] { "Assets/ScriptableObjects" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Item>(path);

                if (asset != null)
                {
                    m_items.Add(asset);
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

        public Item Lookup(Guid id) => m_itemDict.GetValueOrDefault(id);
    }
}
