using UnityEngine;

namespace GlowCore.World
{
    [RequireComponent(typeof(Node))]
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        // Instance Fields
        [Header("Fire VFX")]
        [SerializeField] private ParticleSystem m_fireParticles;
        [SerializeField] private float m_minFireScale = 0.25f;
        [SerializeField] private float m_maxFireScale = 2f;
        [SerializeField] private int m_woodForMaxFire = 5000;

        // Properties
        public static int TotalWoodReceived { get; private set; }

        // Public Methods
        public void FeedWood(int amount)
        {
            if (amount <= 0)
                return;
            TotalWoodReceived += amount;
            UpdateFireScale();
        }

        // Private Methods
        private void Start() => UpdateFireScale();

        private void UpdateFireScale()
        {
            if (m_fireParticles == null)
                return;

            var log = Mathf.Log10(1 + ((float)TotalWoodReceived / m_woodForMaxFire * 9f));
            var t = Mathf.Clamp01(log);
            var scale = Mathf.Lerp(m_minFireScale, m_maxFireScale, t);
            m_fireParticles.transform.localScale = Vector3.one * scale;
        }

        [Header("Debug")]
        [SerializeField][Min(1)] private int m_debugWoodAmount = 1;

        [ContextMenu("Debug: Feed Wood")]
        private void DebugFeedWood()
        {
            FeedWood(m_debugWoodAmount);
        }
    }
}
