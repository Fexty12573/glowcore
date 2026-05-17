using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class TitleCameraDrift : MonoBehaviour
    {
        [SerializeField] private float m_amplitudePos = 0.6f;
        [SerializeField] private float m_amplitudeRot = 1.5f;
        [SerializeField] private float m_speed = 0.25f;

        private Vector3 m_basePos;
        private Vector3 m_baseRot;

        private void Start()
        {
            m_basePos = transform.position;
            m_baseRot = transform.eulerAngles;
        }

        private void Update()
        {
            var t = Time.unscaledTime * m_speed;
            transform.position = m_basePos + new Vector3(
                Mathf.Sin(t) * m_amplitudePos * 0.5f,
                Mathf.Sin(t * 0.6f) * m_amplitudePos * 0.2f,
                Mathf.Cos(t * 0.7f) * m_amplitudePos * 0.4f);
            transform.eulerAngles = m_baseRot + new Vector3(
                Mathf.Sin(t * 0.5f) * m_amplitudeRot * 0.5f,
                Mathf.Cos(t * 0.4f) * m_amplitudeRot,
                0f);
        }
    }
}
