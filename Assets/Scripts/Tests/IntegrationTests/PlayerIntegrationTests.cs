using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerIntegrationTests
{
    private GameObject m_playerPrefab;
    private GameObject m_playerInstance;
    private PlayerMovement m_playerMovement;
    private PlayerCamera m_playerCamera;

    [SetUp]
    public void SetUp()
    {
        m_playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        m_playerInstance = GameObject.Instantiate(m_playerPrefab);
        m_playerMovement = m_playerInstance.GetComponentInChildren<PlayerMovement>();
        m_playerCamera = m_playerInstance.GetComponentInChildren<PlayerCamera>();
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(m_playerInstance);
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
        Assert.AreEqual(oldPosition, newPosition);
    }

    [UnityTest]
    public IEnumerator PlayerCameraMovesOnInput()
    {
        Vector2 lookInput = new(5, -9);
        Quaternion oldRotation = m_playerCamera.transform.rotation;

        m_playerCamera.HandleLook(lookInput);
        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        Assert.AreNotEqual(oldRotation, newRotation);
    }

    [UnityTest]
    public IEnumerator PlayerCameraDoesntChangeOnNoInput()
    {
        Quaternion oldRotation = m_playerCamera.transform.rotation;

        yield return new WaitForFixedUpdate();

        Quaternion newRotation = m_playerCamera.transform.rotation;
        Assert.AreEqual(oldRotation, newRotation);
    }
}
