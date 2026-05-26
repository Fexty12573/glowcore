#if UNITY_EDITOR
using UnityEngine;

namespace Editors.IconGeneration
{
    public static class IconFraming
    {
        public static bool TryCalculateBounds(GameObject target, out Bounds bounds)
        {
            bounds = new Bounds();
            if (target == null)
                return false;

            var renderers = target.GetComponentsInChildren<Renderer>();
            bool initialized = false;
            foreach (var renderer in renderers)
            {
                if (!renderer.enabled)
                    continue;

                if (!initialized)
                {
                    bounds = renderer.bounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return initialized;
        }

        public static void FrameForCamera(GameObject target, Camera camera, float aspect, float fillFraction)
        {
            if (camera == null || !TryCalculateBounds(target, out var bounds))
                return;

            CenterOnCameraAxis(target, camera, bounds);

            // Recompute bounds after moving the target so the framing reflects the new world position.
            if (!TryCalculateBounds(target, out bounds))
                return;

            float safeAspect = Mathf.Max(0.0001f, aspect);
            float safeFill = Mathf.Max(0.0001f, fillFraction);
            if (camera.orthographic)
                FrameOrthographic(camera, bounds, safeAspect, safeFill);
            else
                FramePerspective(camera, bounds, safeAspect, safeFill);
        }

        private static void CenterOnCameraAxis(GameObject target, Camera camera, Bounds bounds)
        {
            Vector3 cameraForward = camera.transform.forward;
            Vector3 cameraPos = camera.transform.position;

            // Keep the existing distance from the camera along its forward axis and slide the target
            // so its bounds center lies on that axis.
            float existingDistance = Vector3.Dot(bounds.center - cameraPos, cameraForward);
            Vector3 focusPoint = cameraPos + (cameraForward * existingDistance);
            Vector3 offset = focusPoint - bounds.center;
            target.transform.position += offset;
        }

        private static void FrameOrthographic(Camera camera, Bounds bounds, float aspect, float fillFraction)
        {
            Vector3 extents = bounds.extents;
            Vector3 right = camera.transform.right;
            Vector3 up = camera.transform.up;

            float halfWidth = ProjectedHalfExtent(extents, right);
            float halfHeight = ProjectedHalfExtent(extents, up);

            float requiredOrthoSize = Mathf.Max(halfHeight, halfWidth / aspect);
            camera.orthographicSize = requiredOrthoSize / fillFraction;
        }

        private static void FramePerspective(Camera camera, Bounds bounds, float aspect, float fillFraction)
        {
            Vector3 extents = bounds.extents;
            Vector3 right = camera.transform.right;
            Vector3 up = camera.transform.up;
            Vector3 forward = camera.transform.forward;

            float halfWidth = ProjectedHalfExtent(extents, right);
            float halfHeight = ProjectedHalfExtent(extents, up);
            float halfDepth = ProjectedHalfExtent(extents, forward);

            float halfFovRad = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float tanHalfFov = Mathf.Tan(halfFovRad);

            float distForHeight = halfHeight / tanHalfFov;
            float distForWidth = (halfWidth / aspect) / tanHalfFov;
            float requiredDistance = (Mathf.Max(distForHeight, distForWidth) / fillFraction) + halfDepth;

            camera.transform.position = bounds.center - (forward * requiredDistance);
        }

        private static float ProjectedHalfExtent(Vector3 extents, Vector3 axis)
        {
            return (Mathf.Abs(axis.x) * extents.x)
                + (Mathf.Abs(axis.y) * extents.y)
                + (Mathf.Abs(axis.z) * extents.z);
        }
    }
}
#endif
