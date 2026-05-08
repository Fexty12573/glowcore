using GlowCore.UI.Menus;
using UnityEngine;
using UnityEngine.InputSystem;

/* Explanation of the Camera Setup:
 The player prefab has the following (simplified) hierarchy:
    Player Root
    ├─ Camera Anchor
    │  └─ Main Camera
    └─ Player Controller
       └─ Player 3D Model

The idea is that the camera orbits around the player.
The camera anchor is at the same position as the player and serves as the center of the orbit.
The orbit can be moved with the m_cameraOffset in PlayerMovement.cs. 
Inspector transforms:
* The Position of Camera Anchor should be set to the same value as m_cameraOffset.
* The Position and Rotation of Main Camera should both be (0, 0, 0)
 */
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform m_cameraAnchor;
    [SerializeField] private float m_horizontalCameraSpeed = 50;
    [SerializeField] private float m_verticalCameraSpeed = 40;
    [SerializeField] private float m_minPitch = 10f; // Pitch is the angle when looking up and down
    [SerializeField] private float m_maxPitch = 89f;

    private Vector2 m_lookInput;
    private float m_sensitivityMultiplier = 1f;
    private IDisplayService m_displayService;

    public void Initialize(IDisplayService displayService)
    {
        UnsubscribeFrom(m_displayService);

        m_displayService = displayService;

        if (m_displayService != null)
        {
            m_displayService.OnMouseSensitivityChanged += OnMouseSensitivityChanged;
            m_sensitivityMultiplier = DisplayService.SliderToMultiplier(m_displayService.MouseSensitivity);
        }
    }

    public void HandleLook(Vector2 lookInput)
    {
        m_lookInput = lookInput;
    }

    private void OnDestroy() => UnsubscribeFrom(m_displayService);

    private void UnsubscribeFrom(IDisplayService service)
    {
        if (service != null)
            service.OnMouseSensitivityChanged -= OnMouseSensitivityChanged;
    }

    private void OnMouseSensitivityChanged(float slider)
    {
        m_sensitivityMultiplier = DisplayService.SliderToMultiplier(slider);
    }

    private void OnLook(InputValue inputValue)
    {
        HandleLook(inputValue.Get<Vector2>());
    }

    private void FixedUpdate()
    {
        var yaw = m_lookInput.x * Time.fixedDeltaTime * m_horizontalCameraSpeed * m_sensitivityMultiplier;
        var pitch = -m_lookInput.y * Time.fixedDeltaTime * m_verticalCameraSpeed * m_sensitivityMultiplier;

        var yawDegrees = yaw + m_cameraAnchor.localEulerAngles.y;
        var pitchDegrees = Mathf.Clamp(pitch + m_cameraAnchor.localEulerAngles.x, m_minPitch, m_maxPitch);

        Quaternion target = Quaternion.Euler(pitchDegrees, yawDegrees, 0);
        m_cameraAnchor.localRotation = Quaternion.Slerp(m_cameraAnchor.localRotation, target, 15 * Time.fixedDeltaTime);
        m_cameraAnchor.eulerAngles = new Vector3(m_cameraAnchor.eulerAngles.x, m_cameraAnchor.eulerAngles.y, 0f); // Remove rotation around z axis
    }
}
