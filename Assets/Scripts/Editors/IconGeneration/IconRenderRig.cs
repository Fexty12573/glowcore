#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editors.IconGeneration
{
    [DisallowMultipleComponent]
    public class IconRenderRig : MonoBehaviour
    {
        [SerializeField] private Camera m_camera;
        [SerializeField] private Transform m_itemAnchor;
        [SerializeField, Range(0, 31)] private int m_iconCaptureLayer = 31;

        public Camera Camera => m_camera;
        public Transform ItemAnchor => m_itemAnchor;
        public int CaptureLayer => m_iconCaptureLayer;

        public GameObject MountPrefab(GameObject prefab)
        {
            if (prefab == null || m_itemAnchor == null)
                return null;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, m_itemAnchor);
            if (instance == null)
                return null;

            // Preserve the prefab's authored rotation and scale — that's what world spawns use.
            // Position is reset to zero so IconFraming can re-center against the camera axis.
            instance.transform.localPosition = Vector3.zero;

            SetLayerRecursive(instance, m_iconCaptureLayer);
            return instance;
        }

        public void Unmount(GameObject instance)
        {
            if (instance != null)
                DestroyImmediate(instance);
        }

        public Texture2D Capture(int renderWidth, int renderHeight, int targetWidth, int targetHeight)
        {
            if (m_camera == null)
                return null;

            var rtSuper = RenderTexture.GetTemporary(
                renderWidth,
                renderHeight,
                24,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB,
                4);
            var rtFinal = RenderTexture.GetTemporary(
                targetWidth,
                targetHeight,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB,
                1);

            rtSuper.filterMode = FilterMode.Bilinear;
            rtFinal.filterMode = FilterMode.Bilinear;

            RenderTexture previousActive = RenderTexture.active;
            RenderTexture previousTarget = m_camera.targetTexture;
            float previousAspect = m_camera.aspect;
            bool aspectWasOverridden = false;

            try
            {
                m_camera.targetTexture = rtSuper;
                m_camera.aspect = (float)targetWidth / Mathf.Max(1, targetHeight);
                aspectWasOverridden = true;

                m_camera.Render();

                Graphics.Blit(rtSuper, rtFinal);

                RenderTexture.active = rtFinal;
                var texture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false, false);
                texture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
                texture.Apply();
                return texture;
            }
            finally
            {
                m_camera.targetTexture = previousTarget;
                if (aspectWasOverridden)
                    m_camera.ResetAspect();

                RenderTexture.active = previousActive;
                RenderTexture.ReleaseTemporary(rtSuper);
                RenderTexture.ReleaseTemporary(rtFinal);
            }
        }

        private static void SetLayerRecursive(GameObject root, int layer)
        {
            root.layer = layer;
            foreach (Transform child in root.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }
}
#endif
