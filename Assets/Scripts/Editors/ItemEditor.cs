#if UNITY_EDITOR
using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(Item), editorForChildClasses: true)]
    public class ItemEditor : Editor
    {
        private const string kWarningMessage = "This item already has an Id, are you sure you want to generate a new one?";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (target is not Item item)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Id", EditorStyles.boldLabel);
            GUILayout.TextArea(item.Id.ToString());
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Id"))
            {
                if (item.Id == Guid.Empty)
                {
                    Undo.RecordObject(item, "Generate Id");
                    item.RegenerateId();
                    EditorUtility.SetDirty(item);
                    return;
                }

                // If the item already has an Id, ask the user if they want to generate a new one
                if (!EditorUtility.DisplayDialog("Generate Id", kWarningMessage, "Yes", "No"))
                    return;

                Undo.RecordObject(item, "Generate Id");
                item.RegenerateId();
                EditorUtility.SetDirty(item);
            }
        }
    }
}

#endif
