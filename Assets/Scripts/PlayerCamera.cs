using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform CameraAnchor;
    [SerializeField]
    private float CameraSpeed = 40;
    
    private Vector2 m_lookInput;
    
    private void FixedUpdate()
    {
        float rotateDegrees = m_lookInput.x * Time.fixedDeltaTime  * CameraSpeed;
        CameraAnchor.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    private void OnLook(InputValue inputValue)
    {
        m_lookInput = inputValue.Get<Vector2>();
    }
    
    
}
