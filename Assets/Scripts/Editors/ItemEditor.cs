#if UNITY_EDITOR
using System;
using Editors.IconGeneration;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(Item), editorForChildClasses: true)]
    public class ItemEditor : Editor
    {
        private const string kWarningMessage = "This item already has an Id, are you sure you want to generate a new one?";
        private const int kPreviewSize = 220;

        private PreviewRenderUtility m_preview;
        private GameObject m_previewInstance;
        private GameObject m_cachedPrefab;
        private Quaternion m_authoredRotation;
        private bool m_showPreview = true;

        private void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += CleanupPreview;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= CleanupPreview;
            CleanupPreview();
        }

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

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(item.Prefab == null))
            {
                if (GUILayout.Button("Generate Icon"))
                {
                    CleanupPreview();
                    Item captured = item;
                    EditorApplication.delayCall += () => IconCaptureService.CaptureIconFor(captured);
                }
            }

            EditorGUILayout.Space();
            m_showPreview = EditorGUILayout.Foldout(m_showPreview, "Icon Preview", true);
            if (m_showPreview)
                DrawIconPreview(item);
        }

        private void DrawIconPreview(Item item)
        {
            if (item.Prefab == null)
            {
                EditorGUILayout.HelpBox("Assign a Prefab to enable the preview.", MessageType.Info);
                return;
            }

            EnsurePreview();
            EnsureInstance(item.Prefab);

            m_previewInstance.transform.localPosition = Vector3.zero;
            m_previewInstance.transform.localRotation = Quaternion.Euler(item.IconRotation) * m_authoredRotation;

            IconFraming.FrameForCamera(m_previewInstance, m_preview.camera, aspect: 1f, fillFraction: 0.85f);

            Rect rect = GUILayoutUtility.GetRect(kPreviewSize, kPreviewSize, GUILayout.ExpandWidth(false));
            m_preview.BeginPreview(rect, GUIStyle.none);
            m_preview.Render(true);
            Texture tex = m_preview.EndPreview();
            GUI.DrawTexture(rect, tex, ScaleMode.StretchToFill, alphaBlend: false);

            EditorGUILayout.HelpBox(
                "Tweak Icon Rotation above - the preview updates live. Background gray is for visibility only; the saved icon has a transparent background.",
                MessageType.None);
        }

        private void EnsurePreview()
        {
            if (m_preview != null)
                return;

            m_preview = new PreviewRenderUtility();

            Camera cam = m_preview.camera;
            cam.orthographic = true;
            cam.orthographicSize = 0.6f;
            cam.transform.position = new Vector3(3f, 0f, -3f);
            cam.transform.rotation = Quaternion.Euler(0f, -45f, 0f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 50f;

            m_preview.lights[0].type = LightType.Directional;
            m_preview.lights[0].intensity = 1.2f;
            m_preview.lights[0].color = new Color(1f, 0.97f, 0.92f);
            m_preview.lights[0].transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            m_preview.lights[1].type = LightType.Directional;
            m_preview.lights[1].intensity = 0.45f;
            m_preview.lights[1].color = new Color(0.85f, 0.9f, 1f);
            m_preview.lights[1].transform.rotation = Quaternion.Euler(20f, 150f, 0f);
        }

        private void EnsureInstance(GameObject prefab)
        {
            if (m_cachedPrefab == prefab && m_previewInstance != null)
                return;

            if (m_previewInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(m_previewInstance);
                m_previewInstance = null;
            }

            m_previewInstance = UnityEngine.Object.Instantiate(prefab);
            m_authoredRotation = m_previewInstance.transform.localRotation;
            m_preview.AddSingleGO(m_previewInstance);
            m_cachedPrefab = prefab;
        }

        private void CleanupPreview()
        {
            if (m_previewInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(m_previewInstance);
                m_previewInstance = null;
            }

            if (m_preview != null)
            {
                m_preview.Cleanup();
                m_preview = null;
            }

            m_cachedPrefab = null;
        }
    }
}

#endif
