using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraAnchor;
    public float movementSpeed = 5;
    public float rotationSpeed = 15;

    private Vector2 _moveInput;

    private void OnMove(InputValue inputValue) // called on press and release
    {
        _moveInput = inputValue.Get<Vector2>();
    }
    
    void Update()
    {
        if (_moveInput != Vector2.zero)
        {
            Vector3 movement3D = new Vector3(_moveInput.x, 0, _moveInput.y);
            Vector3 relativeMovement = cameraAnchor.rotation * movement3D;
            UpdateMovement(relativeMovement);
            UpdateCamera();
            UpdateRotation(relativeMovement);
        }

    }

    private void UpdateMovement(Vector3 movement)
    {
        controller.Move(Time.deltaTime * movementSpeed * movement);
    }

    private void UpdateCamera()
    {
        cameraAnchor.position = transform.position;
    }

    private void UpdateRotation(Vector3 movement)
    {
        Quaternion newDirection = Quaternion.LookRotation(movement);
        Quaternion lerpedDirection = Quaternion.Lerp(transform.rotation, newDirection, Time.deltaTime * rotationSpeed);
        transform.rotation = lerpedDirection;
    }
    
}