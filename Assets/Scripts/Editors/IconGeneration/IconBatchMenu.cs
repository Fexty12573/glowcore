#if UNITY_EDITOR
using System.Collections.Generic;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editors.IconGeneration
{
    public static class IconBatchMenu
    {
        [MenuItem("Tools/GlowCore/Regenerate All Item Icons")]
        private static void RegenerateAll()
        {
            var settings = IconCaptureService.LoadSettings();
            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "Icon Capture",
                    "No IconCaptureSettings asset found.\n\nCreate one via Create → Scriptable Objects → Icon Capture Settings.",
                    "OK");
                return;
            }

            var items = LoadAllItems();
            if (items.Count == 0)
            {
                EditorUtility.DisplayDialog("Icon Capture", "No Item assets found.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog(
                    "Regenerate All Item Icons",
                    $"This will overwrite the Icon field of {items.Count} Item asset(s). Continue?",
                    "Yes",
                    "Cancel"))
                return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Regenerate All Item Icons");

            int success = 0;
            int failed = 0;

            using (var session = IconCaptureService.BeginSession(settings, silent: true))
            {
                if (!session.IsReady)
                {
                    EditorUtility.DisplayDialog("Icon Capture", "Failed to initialize capture session. See console.", "OK");
                    return;
                }

                try
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        bool cancelled = EditorUtility.DisplayCancelableProgressBar(
                            "Regenerating Item Icons",
                            $"({i + 1}/{items.Count}) {item.name}",
                            (float)i / items.Count);

                        if (cancelled)
                            break;

                        if (session.CaptureIcon(item, silent: true))
                            success++;
                        else
                            failed++;
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log($"[IconCapture] Batch complete: {success} succeeded, {failed} failed.");
        }

        private static List<Item> LoadAllItems()
        {
            var list = new List<Item>();
            var guids = AssetDatabase.FindAssets("t:Item");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var item = AssetDatabase.LoadAssetAtPath<Item>(path);
                if (item != null)
                    list.Add(item);
            }
            return list;
        }
    }
}
#endif
