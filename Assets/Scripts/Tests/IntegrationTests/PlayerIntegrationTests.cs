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
        // Arrange
        Vector2 movementInput = new(1, 1);
        Vector3 oldPosition = m_playerMovement.transform.position;
        
        // Act
        m_playerMovement.HandleMove(movementInput);
        yield return new WaitForFixedUpdate(); // FixedUpdate is guaranteed to be called

        // Assert
        Vector3 newPosition = m_playerMovement.transform.position;
        Assert.AreNotEqual(oldPosition, newPosition);
    }

    [UnityTest]
    public IEnumerator PlayerDoesntMoveOnNoInput()
    {
        // Arrange
        Vector3 oldPosition = m_playerMovement.transform.position;
        
        // Act
        yield return new WaitForFixedUpdate(); // FixedUpdate is guaranteed to be called

        // Assert
        Vector3 newPosition = m_playerMovement.transform.position;
        Assert.AreEqual(oldPosition, newPosition);
    }
    
    [UnityTest]
    public IEnumerator PlayerCameraMovesOnInput()
    {
        // Arrange
        Vector2 lookInput = new(5, -9);
        Quaternion oldRotation = m_playerCamera.transform.rotation;
        
        // Act
        m_playerCamera.HandleLook(lookInput);
        yield return new WaitForFixedUpdate(); // FixedUpdate is guaranteed to be called

        // Assert
        Quaternion newRotation = m_playerCamera.transform.rotation;
        Assert.AreNotEqual(oldRotation, newRotation);
    }

    [UnityTest]
    public IEnumerator PlayerCameraDoesntChangeOnNoInput()
    {
        // Arrange
        Quaternion oldRotation = m_playerCamera.transform.rotation;
        
        // Act
        yield return new WaitForFixedUpdate(); // FixedUpdate is guaranteed to be called

        // Assert
        Quaternion newRotation = m_playerCamera.transform.rotation;
        Assert.AreEqual(oldRotation, newRotation);
    }
    
    [UnityTest]
    public IEnumerator IntegrationTestThatShouldFail() // Comment out to test if testing works
    {
        Assert.AreEqual(false, true);
        yield return null;
    }
}
