using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController m_controller;
    [SerializeField] private Transform m_cameraAnchor;
    [SerializeField] private float m_movementSpeed = 5;
    [SerializeField] private float m_rotationSpeed = 12;
    [SerializeField] private Vector3 m_cameraOffset = Vector3.zero;
    [SerializeField] private Animator m_animator;

    private string m_currentState;
    private Vector2 m_moveInput;
    private bool m_isWalkingSoundPlaying;

    public void MultiplyMovementSpeed(float factor) => m_movementSpeed *= factor;

    public void HandleMove(Vector2 movement)
    {
        m_moveInput = movement;
    }

    public void UpdateCamera()
    {
        m_cameraAnchor.position = transform.position + m_cameraOffset; // necessary since the camera is not a child of the player
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
            Vector3 relativeMovement = Quaternion.Euler(0, m_cameraAnchor.eulerAngles.y, 0) * movement3D;
            UpdateMovement(relativeMovement);
            UpdateCamera();
            UpdateRotation(relativeMovement);
            ChangeAnimatorState("walk");
            if (!m_isWalkingSoundPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.Play(
                    AudioManager.SoundType.Walk,
                    AudioManager.AudioChannel.Player);
                m_isWalkingSoundPlaying = true;
            }
        }
        else
        {
            ChangeAnimatorState("idle");

            if (m_isWalkingSoundPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.Stop(
                    AudioManager.AudioChannel.Player);
                m_isWalkingSoundPlaying = false;
            }
        }
    }

    private void ChangeAnimatorState(string state)
    {
        if (state == m_currentState)
            return;

        m_animator.CrossFade(state, 0.1f);
        m_currentState = state;
    }

    private void UpdateMovement(Vector3 movement)
    {
        movement += Vector3.down; // this brings the player back to the ground
        m_controller.Move(Time.fixedDeltaTime * m_movementSpeed * movement);
    }

    private void UpdateRotation(Vector3 movement)
    {
        var newDirection = Quaternion.LookRotation(movement);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, newDirection,
                Time.fixedDeltaTime * m_rotationSpeed); // makes the player turn around smoothly
    }
}