using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ScriptableObjects;

public class CraftingIntegrationTests
{
    private GameObject m_playerInstance;
    private PlayerInventory m_inventory;
    private CraftingSystem m_crafting;

    private Item m_wood;
    private Item m_stick;
    private Recipe m_recipe;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        var playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Prefabs/Player.prefab");
        Assert.NotNull(playerPrefab, "Player prefab not found");
        m_playerInstance = GameObject.Instantiate(playerPrefab);
        yield return new WaitForFixedUpdate();
        m_inventory = m_playerInstance.GetComponentInChildren<PlayerInventory>();
        Assert.NotNull(m_inventory, "PlayerInventory not found");
        var dummyPrefab = new GameObject("DummyPrefab");

        m_wood = ScriptableObject.CreateInstance<Item>();
        m_wood.Name = "Wood";
        m_wood.MaxStack = 99;
        m_wood.Prefab = dummyPrefab;

        m_stick = ScriptableObject.CreateInstance<Item>();
        m_stick.Name = "Stick";
        m_stick.MaxStack = 99;
        m_stick.Prefab = dummyPrefab;

        m_recipe = ScriptableObject.CreateInstance<Recipe>();
        m_recipe.Ingredients = new Recipe.Ingredient[] { new Recipe.Ingredient { Item = m_wood, Amount = 2 } };
        m_recipe.ResultItem = m_stick;
        m_recipe.ResultAmount = 1;

        m_crafting = new CraftingSystem(m_inventory, new[] { m_recipe });

        yield return new WaitForFixedUpdate();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        m_crafting.Dispose();

        Object.DestroyImmediate(m_playerInstance);
        Object.Destroy(m_wood);
        Object.Destroy(m_stick);
        Object.Destroy(m_recipe);

        var field = typeof(NodeActionSystem).GetField("s_instance",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        field.SetValue(null, null);

        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Crafting_RemovesIngredients_And_AddsResult()
    {
        m_inventory.AddItem(m_wood, 2);

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(2, m_inventory.CountItem(m_wood));
        Assert.AreEqual(0, m_inventory.CountItem(m_stick));

        bool canCraft = m_crafting.CanCraft(m_recipe);
        bool crafted = m_crafting.Craft(m_recipe);

        yield return new WaitForFixedUpdate();

        Assert.IsTrue(canCraft, "Should be able to craft");
        Assert.IsTrue(crafted, "Craft succeed");

        Assert.AreEqual(0, m_inventory.CountItem(m_wood), "Wood consumed");
        Assert.AreEqual(1, m_inventory.CountItem(m_stick), "Stick added");
    }

    [UnityTest]
    public IEnumerator Crafting_Fails_When_NotEnoughResources()
    {
        m_inventory.AddItem(m_wood, 1);

        yield return new WaitForFixedUpdate();

        bool canCraft = m_crafting.CanCraft(m_recipe);
        bool crafted = m_crafting.Craft(m_recipe);

        yield return new WaitForFixedUpdate();

        Assert.IsFalse(canCraft, "Should NOT be able to craft");
        Assert.IsFalse(crafted, "Craft fail");

        Assert.AreEqual(1, m_inventory.CountItem(m_wood));
        Assert.AreEqual(0, m_inventory.CountItem(m_stick));
    }
}