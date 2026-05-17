#if UNITY_EDITOR
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(ItemRegistry))]
    public class ItemRegistryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Rebuild Registry"))
            {
                if (target is ItemRegistry registry)
                {
                    registry.RebuildRegistry();
                    EditorUtility.SetDirty(registry);
                }
            }

            DrawDefaultInspector();
        }
    }
}

#endif
