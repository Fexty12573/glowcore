using UnityEngine;

namespace GlowCore.World
{
    [RequireComponent(typeof(Node))]
    [RequireComponent(typeof(Collider))]
    public class Fire : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_fireParticles;
        [SerializeField] private ParticleSystem m_secondaryFireParticles;
        [SerializeField] private float m_fireScale = 1f;
        [SerializeField] private float m_particleAmount = 10f;

        private void Start()
        {
            m_fireParticles.transform.localScale = Vector3.one * m_fireScale;

            var fireEmission = m_fireParticles.emission;
            fireEmission.rateOverTime = m_particleAmount;

            var secondaryEmission = m_secondaryFireParticles.emission;
            secondaryEmission.rateOverTime = m_particleAmount;
        }
    }
}
