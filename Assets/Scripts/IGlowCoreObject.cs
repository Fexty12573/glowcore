using System;
using ScriptableObjects;

namespace GlowCore.World
{
    public interface IGlowCoreObject
    {
        // Data
        GlowCoreLevelConfig LevelConfig { get; }
        GlowCoreLevelConfig NextLevelConfig { get; }
        int Level { get; }
        float TotalProgress01 { get; }
        bool IsReadyToUpgrade { get; }
        bool HasNextLevel { get; }

        // Queries
        int AccumulatedFor(Item item);

        // Commands
        void FeedMaterial(Item item, int amount);
        void Upgrade();

        // Events
        event Action OnProgressChanged;
        event Action OnLevelUp;
    }
}
