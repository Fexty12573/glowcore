using System;
using System.Collections.Generic;
using ScriptableObjects;

public interface ICraftingService : IDisposable
{
    // Queries
    IReadOnlyList<Recipe> Recipes { get; }
    bool CanCraft(Recipe recipe);
    int GetItemCount(Item item);

    // Commands
    bool Craft(Recipe recipe);

    // Events
    event Action OnRecipesRefreshed;
}
