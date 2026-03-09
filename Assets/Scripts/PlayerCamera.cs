using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{

    public Transform partToRotate;
    public float cameraSpeed = 40;
    
    private Vector2 _lookInput;
    
    void Update()
    {
        float rotateDegrees = _lookInput.x * Time.deltaTime  * cameraSpeed;
        partToRotate.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    private void OnLook(InputValue inputValue)
    {
        _lookInput = inputValue.Get<Vector2>();
    }
    
    
}
