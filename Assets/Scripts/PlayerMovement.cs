using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_controller;
    [SerializeField]
    private Transform m_cameraAnchor;
    [SerializeField]
    private float m_movementSpeed = 5;
    [SerializeField]
    private float m_rotationSpeed = 15;

    private Vector2 m_moveInput;

    public void HandleMove(Vector2 movement)
    {
        m_moveInput = movement;
    }
    private void OnMove(InputValue inputValue) // called on press and release
    {
        HandleMove(inputValue.Get<Vector2>());
    }

    private void FixedUpdate()
    {
        if (m_moveInput != Vector2.zero)
        {
            Vector3 movement3D = new(m_moveInput.x, 0, m_moveInput.y);
            Vector3 relativeMovement = m_cameraAnchor.rotation * movement3D;
            UpdateMovement(relativeMovement);
            UpdateCamera();
            UpdateRotation(relativeMovement);
        }
    }

    private void UpdateMovement(Vector3 movement)
    {
        m_controller.Move(Time.fixedDeltaTime * m_movementSpeed * movement);
    }

    private void UpdateCamera()
    {
        m_cameraAnchor.position = transform.position; // necessary since the camera is not a child of the player
    }

    private void UpdateRotation(Vector3 movement)
    {
        var newDirection = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Lerp(transform.rotation, newDirection, Time.fixedDeltaTime * m_rotationSpeed); // makes the player turn around smoothly
    }
}