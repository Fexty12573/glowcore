using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform cameraAnchor;
    [SerializeField]
    private float movementSpeed = 5;
    [SerializeField]
    private float rotationSpeed = 15;

    private Vector2 m_moveInput = new Vector2(0, 0);

    private void OnMove(InputValue inputValue) // called on press and release
    {
        m_moveInput = inputValue.Get<Vector2>();
    }
    
    private void FixedUpdate()
    {
        if (m_moveInput != Vector2.zero)
        {
            Vector3 movement3D = new(m_moveInput.x, 0, m_moveInput.y);
            Vector3 relativeMovement = cameraAnchor.rotation * movement3D;
            UpdateMovement(relativeMovement);
            UpdateCamera();
            UpdateRotation(relativeMovement);
        }
    }

    private void UpdateMovement(Vector3 movement)
    {
        controller.Move(Time.fixedDeltaTime * movementSpeed * movement);
    }

    private void UpdateCamera()
    {
        cameraAnchor.position = transform.position; // necessary since the camera is not a child of the player
    }

    private void UpdateRotation(Vector3 movement)
    {
        var newDirection = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Lerp(transform.rotation, newDirection, Time.fixedDeltaTime * rotationSpeed); // makes the player turn around smoothly
    }
    
}