using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController m_controller;
    [SerializeField] private Transform m_cameraAnchor;
    [SerializeField] private float m_movementSpeed = 5;
    [SerializeField] private float m_smoothing = 20f;
    [SerializeField] private float m_rotationSpeed = 12;
    [SerializeField] private Vector3 m_cameraOffset = Vector3.zero;
    [SerializeField] private float m_cameraFollowDelay = 0.1f;
    [SerializeField] private Animator m_animator;

    private string m_currentState;
    private Vector2 m_moveInput;
    private Vector2 m_smoothedInput;
    private bool m_isWalkingSoundPlaying;
    private Vector3 m_cameraVelocity;
    private float m_potionSpeedMultiplier = 1f;

    public float Smoothing
    {
        get => m_smoothing;
        set => m_smoothing = value;
    }

    public float CameraFollowDelay
    {
        get => m_cameraFollowDelay;
        set => m_cameraFollowDelay = value;
    }

    public float PotionSpeedMultiplier
    {
        get => m_potionSpeedMultiplier;
        set => m_potionSpeedMultiplier = value;
    }

    public void HandleMove(Vector2 movement)
    {
        m_moveInput = movement;
    }

    public void SetCameraStartPosition()
    {
        m_cameraAnchor.position = transform.position + m_cameraOffset;
    }

    private void OnMove(InputValue inputValue) // called on press and release
    {
        HandleMove(inputValue.Get<Vector2>());
    }

    private void Update()
    {
        m_smoothedInput = Vector2.Lerp(m_smoothedInput, m_moveInput, m_smoothing * Time.deltaTime);

        if (m_smoothedInput.magnitude >= 0.05f)
        {
            Vector3 movement3D = new(m_smoothedInput.x, 0, m_smoothedInput.y);
            Vector3 relativeMovement = Quaternion.Euler(0, m_cameraAnchor.eulerAngles.y, 0) * movement3D;
            UpdateMovement(relativeMovement);
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
        if (m_moveInput == Vector2.zero)
        {
            ChangeAnimatorState("idle");

            if (m_isWalkingSoundPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.Stop(
                    AudioManager.AudioChannel.Player);
                m_isWalkingSoundPlaying = false;
            }
        }
        UpdateCamera();
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
        m_controller.Move(Time.deltaTime * m_movementSpeed * m_potionSpeedMultiplier * movement);
    }

    private void UpdateCamera() // necessary since the camera is not a child of the player
    {
        Vector3 targetPosition = transform.position + m_cameraOffset;
        m_cameraAnchor.position = Vector3.SmoothDamp(m_cameraAnchor.position, targetPosition, ref m_cameraVelocity, m_cameraFollowDelay);
    }

    private void UpdateRotation(Vector3 movement)
    {
        var newDirection = Quaternion.LookRotation(movement);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, newDirection,
                Time.deltaTime * m_rotationSpeed); // makes the player turn around smoothly
    }
}