#if UNITY_EDITOR
using UnityEngine;

namespace Editors.IconGeneration
{
    [CreateAssetMenu(fileName = "IconCaptureSettings", menuName = "Scriptable Objects/Icon Capture Settings")]
    public class IconCaptureSettings : ScriptableObject
    {
        [Header("Output")]
        [SerializeField] private string m_outputFolder = "Assets/Sprites/Items/Generated";
        [SerializeField] private string m_rigScenePath = "Assets/Scenes/Editor/ItemIconRig.unity";

        [Header("Resolution")]
        [SerializeField, Min(32)] private int m_targetWidth = 256;
        [SerializeField, Min(32)] private int m_targetHeight = 256;
        [SerializeField, Range(1, 4)] private int m_supersample = 2;

        [Header("Framing")]
        [SerializeField, Range(0.1f, 1f)] private float m_fillFraction = 0.85f;

        public string OutputFolder => m_outputFolder;
        public string RigScenePath => m_rigScenePath;
        public int TargetWidth => m_targetWidth;
        public int TargetHeight => m_targetHeight;
        public int Supersample => m_supersample;
        public float FillFraction => m_fillFraction;

        public int RenderWidth => m_targetWidth * m_supersample;
        public int RenderHeight => m_targetHeight * m_supersample;
    }
}
#endif
