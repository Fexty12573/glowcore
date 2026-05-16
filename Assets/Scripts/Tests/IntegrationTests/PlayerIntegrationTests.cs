using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class PlayerIntegrationTests : InputTestFixture
{
    private const float kDelta = 0.0001f;

    private GameObject m_playerPrefab;
    private GameObject m_playerInstance;
    private PlayerMovement m_playerMovement;
    private PlayerCamera m_playerCamera;
    private Mouse m_mouse;

    [SetUp]
    public void SetUp()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
        m_playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        m_playerInstance = GameObject.Instantiate(m_playerPrefab);
        m_playerMovement = m_playerInstance.GetComponentInChildren<PlayerMovement>();
        m_playerCamera = m_playerInstance.GetComponentInChildren<PlayerCamera>();
        m_mouse = InputSystem.AddDevice<Mouse>();
        GameObject am = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Audio/AudioManager.prefab");
        GameObject es = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Audio/EnvironmentSound.prefab");
        GameObject m = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Audio/Music.prefab");
        GameObject ps = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Audio/PlayerSound.prefab");
        GameObject.Instantiate(am);
        GameObject.Instantiate(es);
        GameObject.Instantiate(m);
        GameObject.Instantiate(ps);
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(m_playerInstance);
        var field = typeof(NodeActionSystem).GetField("s_instance",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        field.SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator PlayerMovesOnInput()
    {
        Vector2 movementInput = new(1, 1);
        Vector3 oldPosition = m_playerMovement.transform.position;

        m_playerMovement.HandleMove(movementInput);
        yield return new WaitForFixedUpdate();

        Vector3 newPosition = m_playerMovement.transform.position;
        Assert.AreNotEqual(oldPosition, newPosition);
    }

    [UnityTest]
    public IEnumerator PlayerDoesntMoveOnNoInput()
    {
        Vector3 oldPosition = m_playerMovement.transform.position;

        yield return new WaitForFixedUpdate();

        Vector3 newPosition = m_playerMovement.transform.position;
        float difference = Vector3.Distance(oldPosition, newPosition);
        Assert.AreEqual(0f, difference, kDelta);
    }

    [UnityTest]
    public IEnumerator PlayerCameraMovesOnInputIfRmbIsPressed()
    {
        Vector2 lookInput = new(5, -9);
        Quaternion oldRotation = m_playerCamera.transform.rotation;

        m_playerCamera.HandleLook(lookInput);
        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        Assert.AreNotEqual(oldRotation, newRotation);
    }

    [UnityTest]
    public IEnumerator PlayerCameraDoesntMoveOnNoInputIfRmbIsPressed()
    {
        Quaternion oldRotation = m_playerCamera.transform.rotation;

        InputSystem.QueueStateEvent(Mouse.current,
            new MouseState { delta = Vector2.zero, buttons = 1 << (int)MouseButton.Right });
        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        float difference = Quaternion.Angle(oldRotation, newRotation);
        Assert.AreEqual(0f, difference, kDelta);
    }

    [UnityTest]
    public IEnumerator PlayerCameraDoesntMoveOnInputIfRmbIsNotPressed()
    {
        Vector2 lookInput = new(5, -9);
        Quaternion oldRotation = m_playerCamera.transform.rotation;
        InputSystem.QueueStateEvent(Mouse.current, new MouseState { delta = lookInput });
        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        float difference = Quaternion.Angle(oldRotation, newRotation);
        Assert.AreEqual(0f, difference, kDelta);
    }

    [UnityTest]
    public IEnumerator PlayerCameraDoesntMoveOnNoInput()
    {
        Quaternion oldRotation = m_playerCamera.transform.rotation;

        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        float difference = Quaternion.Angle(oldRotation, newRotation);
        Assert.AreEqual(0f, difference, kDelta);
    }
}