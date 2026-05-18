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
    private float m_sensitivityMultiplierX = 1f;
    private float m_sensitivityMultiplierY = 1f;
    private IDisplayService m_displayService;
    private PlayerInventory m_playerInventory;

    public void Initialize(IDisplayService displayService)
    {
        UnsubscribeFrom(m_displayService);

        m_displayService = displayService;

        if (m_displayService != null)
        {
            m_displayService.OnCameraSensitivityXChanged += OnCameraSensitivityXChanged;
            m_displayService.OnCameraSensitivityYChanged += OnCameraSensitivityYChanged;
            m_sensitivityMultiplierX = DisplayService.SliderToMultiplier(m_displayService.CameraSensitivityX);
            m_sensitivityMultiplierY = DisplayService.SliderToMultiplier(m_displayService.CameraSensitivityY);
        }
    }

    public void HandleLook(Vector2 lookInput)
    {
        // The Look action is bound to RightMouse + Pointer/delta — that same RMB also drives
        // right-click stack splitting in the inventory. Suppress camera rotation while any
        // inventory-style UI is open so splitting doesn't pan the world.
        if (m_playerInventory == null)
            m_playerInventory = FindFirstObjectByType<PlayerInventory>();

        m_lookInput = m_playerInventory != null && m_playerInventory.IsAnyUIOpen
            ? Vector2.zero
            : lookInput;
    }

    private void OnDestroy() => UnsubscribeFrom(m_displayService);

    private void UnsubscribeFrom(IDisplayService service)
    {
        if (service == null)
            return;
        service.OnCameraSensitivityXChanged -= OnCameraSensitivityXChanged;
        service.OnCameraSensitivityYChanged -= OnCameraSensitivityYChanged;
    }

    private void OnCameraSensitivityXChanged(float slider)
    {
        m_sensitivityMultiplierX = DisplayService.SliderToMultiplier(slider);
    }

    private void OnCameraSensitivityYChanged(float slider)
    {
        m_sensitivityMultiplierY = DisplayService.SliderToMultiplier(slider);
    }

    private void OnLook(InputValue inputValue)
    {
        HandleLook(inputValue.Get<Vector2>());
    }

    private void Update()
    {
        var yaw = m_lookInput.x * Time.fixedDeltaTime * m_horizontalCameraSpeed * m_sensitivityMultiplierX;
        var pitch = -m_lookInput.y * Time.fixedDeltaTime * m_verticalCameraSpeed * m_sensitivityMultiplierY;

        var yawDegrees = yaw + m_cameraAnchor.localEulerAngles.y;
        var pitchDegrees = Mathf.Clamp(pitch + m_cameraAnchor.localEulerAngles.x, m_minPitch, m_maxPitch);

        Quaternion target = Quaternion.Euler(pitchDegrees, yawDegrees, 0);
        m_cameraAnchor.localRotation = Quaternion.Slerp(m_cameraAnchor.localRotation, target, 15 * Time.deltaTime);
        m_cameraAnchor.eulerAngles = new Vector3(m_cameraAnchor.eulerAngles.x, m_cameraAnchor.eulerAngles.y, 0f); // Remove rotation around z axis
    }
}
