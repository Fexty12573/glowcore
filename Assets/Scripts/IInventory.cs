namespace GlowCore.Player
{
    public interface IInventory
    {
        int WoodCount { get; }
        void AddWood(int amount);
        int RemoveAllWood();
    }
}
