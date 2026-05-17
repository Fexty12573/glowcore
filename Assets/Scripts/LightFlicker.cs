using UnityEngine;

namespace GlowCore.World
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private Light m_light;
        [SerializeField] private float m_minIntensity = 1.5f;
        [SerializeField] private float m_maxIntensity = 3f;
        [SerializeField] private float m_speed = 10f;

        private float m_seed;

        private void Awake()
        {
            m_seed = Random.Range(0f, 100f);
        }
        private void Update()
        {
            float noise = Mathf.PerlinNoise(m_seed, Time.time * m_speed);
            m_light.intensity = Mathf.Lerp(m_minIntensity, m_maxIntensity, noise);
        }
    }
}