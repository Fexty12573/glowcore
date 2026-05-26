#if UNITY_EDITOR
using System;
using System.IO;
using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editors.IconGeneration
{
    public static class IconCaptureService
    {
        private const string kSettingsTypeFilter = "t:IconCaptureSettings";
        private const string kLogPrefix = "[IconCapture]";

        public static bool CaptureIconFor(Item item)
        {
            var settings = LoadSettings();
            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "Icon Capture",
                    "No IconCaptureSettings asset found.\n\nCreate one via Create → Scriptable Objects → Icon Capture Settings.",
                    "OK");
                return false;
            }

            using var session = BeginSession(settings, silent: false);
            if (!session.IsReady)
                return false;

            return session.CaptureIcon(item, silent: false);
        }

        public static Session BeginSession(IconCaptureSettings settings, bool silent)
        {
            return new Session(settings, silent);
        }

        public static IconCaptureSettings LoadSettings()
        {
            var guids = AssetDatabase.FindAssets(kSettingsTypeFilter);
            if (guids == null || guids.Length == 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<IconCaptureSettings>(path);
        }

        public sealed class Session : IDisposable
        {
            private readonly IconCaptureSettings m_settings;
            private readonly SceneSetup[] m_sceneSetup;
            private readonly IconRenderRig m_rig;
            private readonly bool m_silent;
            private bool m_disposed;

            public bool IsReady => m_rig != null;

            public IconCaptureSettings Settings => m_settings;

            internal Session(IconCaptureSettings settings, bool silent)
            {
                m_settings = settings;
                m_silent = silent;

                if (settings == null)
                    return;

                if (!silent && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return;

                m_sceneSetup = EditorSceneManager.GetSceneManagerSetup();

                Scene rigScene;
                try
                {
                    rigScene = EditorSceneManager.OpenScene(settings.RigScenePath, OpenSceneMode.Additive);
                }
                catch (Exception e)
                {
                    Report($"Failed to open rig scene at '{settings.RigScenePath}': {e.Message}", LogType.Error);
                    return;
                }

                m_rig = FindRig(rigScene);
                if (m_rig == null)
                    Report($"No IconRenderRig found in '{settings.RigScenePath}'.", LogType.Error);
            }

            public bool CaptureIcon(Item item, bool silent)
            {
                if (m_disposed)
                    throw new ObjectDisposedException(nameof(Session));

                if (!IsReady)
                    return false;

                if (!Validate(item, silent))
                    return false;

                GameObject mountedInstance = null;
                Texture2D captured = null;

                try
                {
                    mountedInstance = m_rig.MountPrefab(item.Prefab);
                    if (mountedInstance == null)
                    {
                        Report($"Failed to mount prefab for '{item.name}'.", LogType.Error, silent);
                        return false;
                    }

                    mountedInstance.transform.localRotation = Quaternion.Euler(item.IconRotation) * mountedInstance.transform.localRotation;

                    float aspect = (float)m_settings.TargetWidth / Mathf.Max(1, m_settings.TargetHeight);
                    IconFraming.FrameForCamera(mountedInstance, m_rig.Camera, aspect, m_settings.FillFraction);

                    captured = m_rig.Capture(
                        m_settings.RenderWidth,
                        m_settings.RenderHeight,
                        m_settings.TargetWidth,
                        m_settings.TargetHeight);
                    if (captured == null)
                    {
                        Report($"Capture failed for '{item.name}'.", LogType.Error, silent);
                        return false;
                    }

                    string path = ResolveOutputPath(item, m_settings);
                    EnsureFolderExists(Path.GetDirectoryName(path));

                    File.WriteAllBytes(path, captured.EncodeToPNG());
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                    IconImportPostProcessor.Apply(path, m_settings.TargetWidth);

                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite == null)
                    {
                        Report($"Sprite import failed for '{path}'.", LogType.Error, silent);
                        return false;
                    }

                    Undo.RecordObject(item, "Generate Item Icon");
                    item.Icon = sprite;
                    EditorUtility.SetDirty(item);

                    Debug.Log($"{kLogPrefix} Generated icon for {item.name} → {path}", item);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Report($"Icon capture failed for '{item.name}': {e.Message}", LogType.Error, silent);
                    return false;
                }
                finally
                {
                    if (captured != null)
                        UnityEngine.Object.DestroyImmediate(captured);

                    if (mountedInstance != null)
                        m_rig.Unmount(mountedInstance);
                }
            }

            public void Dispose()
            {
                if (m_disposed)
                    return;
                m_disposed = true;

                try
                {
                    AssetDatabase.SaveAssets();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (m_sceneSetup != null && m_sceneSetup.Length > 0)
                {
                    try
                    {
                        EditorSceneManager.RestoreSceneManagerSetup(m_sceneSetup);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            private bool Validate(Item item, bool silent)
            {
                if (item == null)
                {
                    Report("Item is null.", LogType.Warning, silent);
                    return false;
                }

                if (item.Prefab == null)
                {
                    Report($"Item '{item.name}' has no Prefab assigned.", LogType.Warning, silent);
                    return false;
                }

                if (item.Prefab.GetComponentInChildren<Renderer>() == null)
                {
                    Report($"Prefab '{item.Prefab.name}' has no Renderer in its hierarchy.", LogType.Warning, silent);
                    return false;
                }

                return true;
            }

            private static IconRenderRig FindRig(Scene scene)
            {
                if (!scene.IsValid())
                    return null;

                foreach (var root in scene.GetRootGameObjects())
                {
                    var rig = root.GetComponentInChildren<IconRenderRig>(true);
                    if (rig != null)
                        return rig;
                }
                return null;
            }

            private static string ResolveOutputPath(Item item, IconCaptureSettings settings)
            {
                string safeName = SanitizeFileName(string.IsNullOrEmpty(item.name) ? "UnnamedItem" : item.name);
                return $"{settings.OutputFolder}/{safeName}.png";
            }

            private static string SanitizeFileName(string raw)
            {
                var invalid = Path.GetInvalidFileNameChars();
                var chars = raw.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (Array.IndexOf(invalid, chars[i]) >= 0)
                        chars[i] = '_';
                }
                return new string(chars);
            }

            private static void EnsureFolderExists(string folderPath)
            {
                if (string.IsNullOrEmpty(folderPath))
                    return;

                folderPath = folderPath.Replace('\\', '/');
                if (AssetDatabase.IsValidFolder(folderPath))
                    return;

                string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
                string leaf = Path.GetFileName(folderPath);
                if (!string.IsNullOrEmpty(parent))
                    EnsureFolderExists(parent);

                if (!AssetDatabase.IsValidFolder(folderPath))
                    AssetDatabase.CreateFolder(parent, leaf);
            }

            private void Report(string message, LogType level, bool? silentOverride = null)
            {
                bool isSilent = silentOverride ?? m_silent;
                string full = $"{kLogPrefix} {message}";

                if (level == LogType.Error)
                    Debug.LogError(full);
                else if (level == LogType.Warning)
                    Debug.LogWarning(full);
                else
                    Debug.Log(full);

                if (!isSilent && level == LogType.Error)
                    EditorUtility.DisplayDialog("Icon Capture", message, "OK");
            }
        }
    }
}
#endif
