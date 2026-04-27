using UnityEngine;
using UnityEngine.InputSystem;

// Classes that inherit Consumable only has to implement the effect that happens when Consuming the Item.
// The removing out of the inventory after Consuming is abstracted into here. 
public abstract class Consumable : MonoBehaviour, IHandItem, IPlayerInventoryAware
{
    private PlayerInventory m_inventory;
    public void Use(InputValue inputValue)
    {
        if (inputValue.Get<float>() >= 0.5f)
        {
            ApplyConsumeEffect();
            m_inventory.ConsumeHandItem(1);
        }
    }

    public void SetInventory(PlayerInventory inventory) => m_inventory = inventory;

    protected abstract void ApplyConsumeEffect();
}
