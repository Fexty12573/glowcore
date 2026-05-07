using UnityEngine;

public class SawBlade : MonoBehaviour
{
    [SerializeField] private Transform m_saw;
    [SerializeField] private float m_spinSpeed = 300f;

    private void Update()
    {
        m_saw.transform.Rotate(0f, 0f, m_spinSpeed * Time.deltaTime);
    }
}
