using System;
using System.Collections.Generic;
using ScriptableObjects;

public class CraftingSystem : ICraftingService, IDisposable
{
    private readonly IInventoryService m_inventory;
    private readonly Recipe[] m_recipes;

    public IReadOnlyList<Recipe> Recipes => m_recipes;

    public event Action OnRecipesRefreshed;

    public CraftingSystem(IInventoryService inventory, Recipe[] recipes)
    {
        m_inventory = inventory;
        m_recipes = recipes ?? Array.Empty<Recipe>();
        m_inventory.OnSlotChanged += OnSlotChanged;
    }

    public void Dispose()
    {
        m_inventory.OnSlotChanged -= OnSlotChanged;
    }

    public int GetItemCount(Item item) => m_inventory.CountItem(item);

    public bool CanCraft(Recipe recipe)
    {
        if (recipe == null)
            return false;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (m_inventory.CountItem(ingredient.Item) < ingredient.Amount)
                return false;
        }

        return m_inventory.CanAcceptItem(recipe.ResultItem, recipe.ResultAmount);
    }

    public bool Craft(Recipe recipe)
    {
        if (!CanCraft(recipe))
            return false;

        foreach (var ingredient in recipe.Ingredients)
            m_inventory.RemoveItems(ingredient.Item, ingredient.Amount);

        m_inventory.AddItem(recipe.ResultItem, recipe.ResultAmount);

        AudioManager.Instance.PlayOneShot(AudioManager.SoundType.Craft, AudioManager.AudioChannel.Environment);

        return true;
    }

    private void OnSlotChanged(SlotChangedEvent evt)
    {
        OnRecipesRefreshed?.Invoke();
    }
}