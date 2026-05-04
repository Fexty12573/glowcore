using GlowCore.World;

public class SaveService : ISaveService
{
    public void Save() => WorldGrid.Instance.SaveGameData();
    public void Load() => WorldGrid.Instance.LoadSaveData();
}
