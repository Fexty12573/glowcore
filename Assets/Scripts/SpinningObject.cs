using UnityEngine;

public class SpinningObject : MonoBehaviour
{
    [SerializeField] private Transform m_transform;
    [SerializeField] private Vector3 m_spinSpeed = new(0f, 0f, 100f);

    private void Update()
    {
        m_transform.Rotate(m_spinSpeed * Time.deltaTime);
    }
}
