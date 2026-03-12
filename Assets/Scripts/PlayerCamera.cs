using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_cameraAnchor;
    [SerializeField]
    private float m_cameraSpeed = 40;
    
    private Vector2 m_lookInput;

    public void HandleLook(Vector2 lookInput)
    {
        m_lookInput = lookInput;
    }
    
    private void OnLook(InputValue inputValue)
    {
        HandleLook(inputValue.Get<Vector2>());
    }
    
    private void FixedUpdate()
    {
        float rotateDegrees = m_lookInput.x * Time.fixedDeltaTime * m_cameraSpeed;
        m_cameraAnchor.Rotate(new Vector3(0, rotateDegrees, 0));
    }
}
