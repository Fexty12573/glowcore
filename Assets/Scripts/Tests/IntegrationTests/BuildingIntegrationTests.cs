using System.Collections;
using GlowCore.World;
using NUnit.Framework;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.TestTools;

public class BuildingIntegrationTests
{
    private WorldGrid m_world;
    private BlockBehaviour m_blockBehaviour;

    private PlayerInventory m_inventory;
    private NodeActionSystem m_nodeActionSystem;

    private GameObject m_blockPrefab;
    private GameObject m_nodePrefab;
    private GameObject m_player;

    private Block m_chestItem;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Player.prefab");
        m_player = GameObject.Instantiate(playerPrefab);
        m_player.transform.position = new Vector3(3f, 0f, 1.5f);

        m_nodeActionSystem = m_player.transform.Find("CameraAnchor/MainCamera").GetComponent<NodeActionSystem>();
        m_inventory = m_player.transform.Find("PlayerController/Inventory").GetComponent<PlayerInventory>();
        yield return null;

        GameObject grid = new GameObject("WorldGrid");
        grid.SetActive(false);

        WorldGrid worldGrid = grid.AddComponent<WorldGrid>();

        SetPrivateField(worldGrid, "m_player", m_player.transform.Find("PlayerController"));
        SetPrivateField(worldGrid, "m_gridSize", 10);

        SetPrivateField(worldGrid, "m_borderNorth", new GameObject("N").transform);
        SetPrivateField(worldGrid, "m_borderSouth", new GameObject("S").transform);
        SetPrivateField(worldGrid, "m_borderEast", new GameObject("E").transform);
        SetPrivateField(worldGrid, "m_borderWest", new GameObject("W").transform);

        SetPrivateField(worldGrid, "m_nodesParent", new GameObject("Nodes").transform);
        SetPrivateField(worldGrid, "m_worldMode", WorldMode.DesignedWorld);

        worldGrid.SetSaveService(new MockSaveService());
        grid.SetActive(true);

        GameObject chestPrefab =
            UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Items/Blocks/ChestBlock.prefab");
        m_blockBehaviour = chestPrefab.GetComponent<BlockBehaviour>();
        SetPrivateField(m_blockBehaviour, "m_inventory", m_inventory);
        m_chestItem =
            UnityEditor.AssetDatabase.LoadAssetAtPath<Block>("Assets/ScriptableObjects/Blocks/ChestBlock.asset");
        ItemStack chestStack = new ItemStack(m_chestItem, 5);
        yield return null;

        m_inventory.AddItem(chestStack);
        var inventoryInstance = m_inventory.GetType().GetField("m_inventory",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(m_inventory);
        var itemsField = inventoryInstance.GetType().GetField("m_items",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        ItemStack[] items = (ItemStack[])itemsField.GetValue(inventoryInstance);
        (items[0], items[1]) = (items[1], items[0]);

        yield return null;
    }

    private void InvokePrivateMethod(object target, string methodName, params object[] args)
    {
        var method = target.GetType().GetMethod(methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method == null)
            Assert.Fail($"Methode {methodName} wurde nicht gefunden");
        method.Invoke(target, args);
    }

    [UnityTest]
    public IEnumerator Build_PlaceChest_SpawnsNodeAndReducesInventory()
    {
        Vector2Int targetTile = new Vector2Int(3, 3);

        SetPrivateField(m_nodeActionSystem, "m_currentTile", targetTile);

        InvokePrivateMethod(m_blockBehaviour, "HandleTileChanged", targetTile);

        yield return null;
        InvokePrivateMethod(m_blockBehaviour, "TryToBuild");

        yield return null;

        var isOccupied = WorldGrid.Instance.IsOccupied(targetTile);
        Assert.IsTrue(isOccupied, "Das WorldGrid sollte an der Stelle (3,3) besetzt sein");

        Assert.AreEqual(4, m_inventory.CountItem(m_chestItem));
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(target, value);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.DestroyImmediate(m_player);
        Object.DestroyImmediate(m_world);
        var field = typeof(NodeActionSystem).GetField("s_instance",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        field.SetValue(null, null);

        yield return null;
    }
}