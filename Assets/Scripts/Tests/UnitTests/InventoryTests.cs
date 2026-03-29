using NUnit.Framework;
using ScriptableObjects;
using UnityEngine;

public class InventoryTests
{
    private Item m_item;
    private Item m_otherItem;

    [SetUp]
    public void SetUp()
    {
        m_item = ScriptableObject.CreateInstance<Item>();
        m_item.Name = "Test Item";
        m_item.MaxStack = 10;

        m_otherItem = ScriptableObject.CreateInstance<Item>();
        m_otherItem.Name = "Other Item";
        m_otherItem.MaxStack = 10;
    }

    [Test]
    public void AddItems_ShouldAddToExistingStack()
    {
        var inventory = new Inventory(2, 2);
        inventory[0, 0].Set(m_item, 1);
        var stack = ItemStack.Create(m_item, 5);
        inventory.AddItems(ref stack);

        Assert.AreEqual(inventory[0, 0].Amount, 6);
    }

    [Test]
    public void AddItems_ShouldPlaceStackInFirstEmptySlot_WhenNoExistingStack()
    {
        var inventory = new Inventory(2, 2);
        var stack = new ItemStack(m_item, 3);

        inventory.AddItems(ref stack);

        Assert.AreSame(m_item, inventory[0, 0].Item);
        Assert.AreEqual(3, inventory[0, 0].Amount);
        Assert.AreEqual(0, stack.Amount);
    }

    [Test]
    public void AddItems_ShouldNotUseEmptySlot_WhenExistingStackHandledIt()
    {
        var inventory = new Inventory(2, 2);
        inventory[0, 0].Set(m_item, 1);
        var stack = new ItemStack(m_item, 2);

        inventory.AddItems(ref stack);

        Assert.AreEqual(3, inventory[0, 0].Amount);
        Assert.AreEqual(0, inventory[1, 0].Amount);
        Assert.AreEqual(0, stack.Amount);
    }

    [Test]
    public void AddItems_ShouldFillAllPartialStacksBeforeUsingEmptySlot()
    {
        var inventory = new Inventory(2, 2);
        inventory[0, 0].Set(m_item, 8);
        inventory[1, 0].Set(m_item, 9);
        var stack = new ItemStack(m_item, 5);

        inventory.AddItems(ref stack);

        Assert.AreEqual(m_item.MaxStack, inventory[0, 0].Amount);
        Assert.AreEqual(m_item.MaxStack, inventory[1, 0].Amount);
        Assert.AreEqual(2, inventory[0, 1].Amount + inventory[1, 1].Amount);
        Assert.AreEqual(0, stack.Amount);
    }

    [Test]
    public void GetFirstEmptySlot_ShouldReturnNull_WhenInventoryIsFull()
    {
        var inventory = new Inventory(2, 2);
        inventory[0, 0].Set(m_item, 1);
        inventory[1, 0].Set(m_otherItem, 1);
        inventory[0, 1].Set(m_item, 1);
        inventory[1, 1].Set(m_otherItem, 1);

        var empty = inventory.GetFirstEmptySlot();

        Assert.IsFalse(empty.HasValue);
    }

    [Test]
    public void Indexer_ShouldMap2DCoordinatesCorrectly()
    {
        var inventory = new Inventory(2, 2);

        inventory[1, 0].Set(m_item, 4);
        inventory[0, 1].Set(m_otherItem, 7);

        Assert.AreSame(m_item, inventory[1, 0].Item);
        Assert.AreEqual(4, inventory[1, 0].Amount);
        Assert.AreSame(m_otherItem, inventory[0, 1].Item);
        Assert.AreEqual(7, inventory[0, 1].Amount);
    }
}
