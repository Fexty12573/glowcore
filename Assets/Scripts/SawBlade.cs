using UnityEngine;

public class SawBlade : MonoBehaviour
{
    private enum RotationAxis { X, Y, Z }
    
    [SerializeField] private Transform m_saw;
    [SerializeField] private float m_spinSpeed = 300f;
    [SerializeField] private RotationAxis m_axis = RotationAxis.Z;

    private void Update()
    {
        float speed = m_spinSpeed * Time.deltaTime;

        switch (m_axis)
        {
            case RotationAxis.X:
                m_saw.Rotate(speed, 0f, 0f);
                break;
            case RotationAxis.Y:
                m_saw.Rotate(0f, speed, 0f);
                break;
            case RotationAxis.Z:
                m_saw.Rotate(0f, 0f, speed);
                break;
        }
    }
}
